using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Metamong.Core;

public class FastAPIHandler
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void NotifyUnityReady();

    [DllImport("__Internal")]
    private static extern void LogoutFromBrowser();
#endif

    private MonoBehaviour _runner;
    private string BACKEND_URL = "";
    // 엔드포인트 상수
    //private const string BACKEND_URL = "http://localhost:8000";
    private const string EP_ME = "/api/users/me";
    private const string EP_ME_RC = "/api/users/me/rc";
    private const string EP_ME_INIT = "/api/users/me/initialize";
    // Newtonsoft 직렬화 설정 (snake_case → C# 프로퍼티)
    private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
    {
        ContractResolver = new DefaultContractResolver(),
        NullValueHandling = NullValueHandling.Ignore
    };

    // 인증 상태 
    public string AccessToken { get; private set; }
    public UserData CurrentUser { get; private set; }
    public bool IsLoggedIn => !string.IsNullOrEmpty(AccessToken) && CurrentUser != null;

    // 이벤트
    public event Action<UserData> OnLoginComplete;
    public event Action<UserData> OnUserUpdated;
    public event Action OnLogout;
    public event Action<string> OnLoginFailed;

    // 초기화 -----------------------------------------------------
    public FastAPIHandler(MonoBehaviour runner, string FastAPIUrl)
    {
        _runner = runner;
        BACKEND_URL = FastAPIUrl;
#if UNITY_WEBGL && !UNITY_EDITOR
        NotifyUnityReady();
#endif
    }

    /*public void Init()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        NotifyUnityReady();
#endif
    }*/

    // index.html → SendMessage("Managers", "OnReceiveAuthToken", token)
    public void ReceiveToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return;

        AccessToken = token;
        Debug.Log("[Auth] 토큰 수신, /users/me 요청");
        //Managers.Instance.StartCoroutine(FetchMe());
        _runner.StartCoroutine(FetchMe());
    }

    // /users/me -----------------------------------------------------

    private IEnumerator FetchMe()
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{BACKEND_URL}{EP_ME}"))
        {
            www.SetRequestHeader("Authorization", $"Bearer {AccessToken}");
            yield return www.SendWebRequest();

            switch (www.responseCode)
            {
                case 200:
                    if (TryParseUser(www.downloadHandler.text, out UserData user))
                    {
                        CurrentUser = user;
                        Debug.Log($"[Auth] 로그인 완료 | {user.Nickname} " +
                                  $"({user.AuthProvider}) | 상태: {user.Status}");

                        if (user.IsNewUser)
                            Debug.Log("[Auth] 신규 유저 - 초기 프로필 설정 필요");

                        OnLoginComplete?.Invoke(CurrentUser);
                    }
                    break;

                case 401:
                    Debug.LogWarning("[Auth] 토큰 만료 또는 무효");
                    HandleAuthFailure("Token expired");
                    break;

                case 404:
                    Debug.LogError("[Auth] 유저를 찾을 수 없음");
                    HandleAuthFailure("User not found");
                    break;

                default:
                    Debug.LogError($"[Auth] /users/me 실패: {www.responseCode}");
                    HandleAuthFailure($"Server error: {www.responseCode}");
                    break;
            }
        }
    }

    // /users/me/initialize (NEW 유저 초기화) -----------------------------------------------------

    /// <summary>
    /// NEW 상태 유저의 최초 프로필 설정
    /// 완료 후 status가 ACTIVE로 변경됨
    /// </summary>
    public IEnumerator InitializeUser(
        InitializeUserRequest initData,
        Action<UserData> onSuccess = null,
        Action<string> onError = null)
    {
        if (!IsLoggedIn) { onError?.Invoke("Not logged in"); yield break; }
        if (!CurrentUser.IsNewUser) { onError?.Invoke("User is already initialized"); yield break; }

        //yield return Managers.Instance.StartCoroutine(
        yield return _runner.StartCoroutine(
            Patch(EP_ME_INIT, initData,
                onSuccess: json =>
                {
                    if (TryParseUser(json, out UserData updated))
                    {
                        CurrentUser = updated;
                        Debug.Log($"[Auth] 초기화 완료 | RC: {updated.Rc} | 상태: {updated.Status}");
                        OnUserUpdated?.Invoke(CurrentUser);
                        onSuccess?.Invoke(CurrentUser);
                    }
                },
                onError: onError
            )
        );
    }

    // /users/me/rc (RC 변경) -----------------------------------------------------

    /// <summary>RC 업데이트</summary>
    public IEnumerator UpdateRC(
        RC newRc,
        Action<UserData> onSuccess = null,
        Action<string> onError = null)
    {
        if (!IsLoggedIn) { onError?.Invoke("Not logged in"); yield break; }

        var body = new RCUpdateRequest(newRc);

        //yield return Managers.Instance.StartCoroutine(
        yield return _runner.StartCoroutine(
            Patch(EP_ME_RC, body,
                onSuccess: json =>
                {
                    if (TryParseUser(json, out UserData updated))
                    {
                        CurrentUser = updated;
                        Debug.Log($"[Auth] RC 변경 완료: {updated.Rc}");
                        OnUserUpdated?.Invoke(CurrentUser);
                        onSuccess?.Invoke(CurrentUser);
                    }
                },
                onError: onError
            )
        );
    }

    // 공통 HTTP 헬퍼 -----------------------------------------------------

    public IEnumerator Get(
        string endpoint,
        Action<string> onSuccess,
        Action<string> onError = null)
    {
        if (!IsLoggedIn) { onError?.Invoke("Not logged in"); yield break; }

        using (UnityWebRequest www = UnityWebRequest.Get($"{BACKEND_URL}{endpoint}"))
        {
            www.SetRequestHeader("Authorization", $"Bearer {AccessToken}");
            yield return www.SendWebRequest();

            HandleResponse(www, onSuccess, onError);
        }
    }

    public IEnumerator Post(
        string endpoint,
        object body,
        Action<string> onSuccess,
        Action<string> onError = null)
    {
        if (!IsLoggedIn) { onError?.Invoke("Not logged in"); yield break; }

        //yield return Managers.Instance.StartCoroutine(
        yield return _runner.StartCoroutine(
            SendJson("POST", endpoint, body, onSuccess, onError)
        );
    }

    private IEnumerator Patch(
        string endpoint,
        object body,
        Action<string> onSuccess,
        Action<string> onError = null)
    {
        //yield return Managers.Instance.StartCoroutine(
        yield return _runner.StartCoroutine(
            SendJson("PATCH", endpoint, body, onSuccess, onError)
        );
    }

    private IEnumerator SendJson(
        string method,
        string endpoint,
        object body,
        Action<string> onSuccess,
        Action<string> onError)
    {
        string json = JsonConvert.SerializeObject(body, _jsonSettings);
        byte[] raw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest www = new UnityWebRequest($"{BACKEND_URL}{endpoint}", method))
        {
            www.uploadHandler = new UploadHandlerRaw(raw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", $"Bearer {AccessToken}");
            yield return www.SendWebRequest();

            HandleResponse(www, onSuccess, onError);
        }
    }

    private void HandleResponse(
        UnityWebRequest www,
        Action<string> onSuccess,
        Action<string> onError)
    {
        if (www.result == UnityWebRequest.Result.Success)
            onSuccess?.Invoke(www.downloadHandler.text);
        else
            onError?.Invoke($"{www.responseCode}: {www.error}");
    }

    // 로그아웃 -----------------------------------------------------

    public void Logout()
    {
        ClearAuth();
        OnLogout?.Invoke();

#if UNITY_WEBGL && !UNITY_EDITOR
        LogoutFromBrowser();
#endif
    }

    // 내부 유틸 -----------------------------------------------------

    private bool TryParseUser(string json, out UserData user)
    {
        user = null;
        try
        {
            user = JsonConvert.DeserializeObject<UserData>(json, _jsonSettings);
            return user != null;
        }
        catch (JsonException e)
        {
            Debug.LogError($"[Auth] UserData 역직렬화 실패: {e.Message}\n{json}");
            return false;
        }
    }

    private void HandleAuthFailure(string reason)
    {
        ClearAuth();
        OnLoginFailed?.Invoke(reason);
    }

    private void ClearAuth()
    {
        AccessToken = null;
        CurrentUser = null;
    }
}
