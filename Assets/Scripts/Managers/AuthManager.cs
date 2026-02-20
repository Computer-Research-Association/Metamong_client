using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Metamong.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.Networking;

public class AuthManager
{
    // js lib import
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void NotifyUnityReady();
    
    [DllImport("__Internal")]
    private static extern void LogoutFromBrowser();
    
    [DllImport("__Internal")]
    private static extern string GetStoredToken();
#endif

    private const string BACKEND_URL = "http://localhost:8000";

    // 엔드포인트 상수
    private const string EP_ME = "/api/users/me";
    private const string EP_ME_RC = "/api/users/me/rc";
    private const string EP_ME_INIT = "/api/users/me/initialize";

    // Newtonsoft 직렬화 설정 (snake_case → C# 프로퍼티)
    private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
    {
        ContractResolver = new DefaultContractResolver(),
        NullValueHandling = NullValueHandling.Ignore
    };

    // 토큰 및 유저 정보
    public string AccessToken { get; private set; }
    public UserData CurrentUser { get; private set; }
    public bool IsLoggedIn => !string.IsNullOrEmpty(AccessToken) && CurrentUser != null;

    // 이벤트
    public event Action<UserData> OnLoginComplete;
    public event Action OnLogout;
    public event Action<string> OnLoginFailed;
    public event Action<UserData> OnUserUpdated;

    public void Init()
    {
        // Unity 로딩 완료를 웹 페이지에 알려서 토큰 주입 요청
#if UNITY_WEBGL && !UNITY_EDITOR
        NotifyUnityReady();
#elif UNITY_EDITOR
        Debug.Log("[AuthManager] Init()");
#endif

    }

    // index.html의 SendMessage로 호출됨
    // Managers.cs에서 라우팅
    public void ReceiveToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return;

        AccessToken = token;
        Debug.Log("Token received from web page");

        // 코루틴은 MonoBehaviour에서만 가능 -> Managers에 위임
        Managers.Instance.StartCoroutine(VerifyToken(token));
    }

    private IEnumerator VerifyToken(string token)
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{BACKEND_URL}/api/auth/verify"))
        {
            www.SetRequestHeader("Authorization", $"Bearer {token}");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // VerifyResponse 역직렬화 (UserData 포함)
                var response = JsonConvert.DeserializeObject<VerifyResponse>(
                    www.downloadHandler.text
                );

                if (response.Valid)
                {
                    CurrentUser = response.User;
                    Debug.Log($"[Auth] 인증 완료 | {CurrentUser.Nickname} ({CurrentUser.AuthProvider})");
                    OnLoginComplete?.Invoke(CurrentUser);
                }
                else
                {
                    ClearToken();
                    OnLoginFailed?.Invoke("Invalid token");
                }
            }
            else
            {
                ClearToken();
                OnLoginFailed?.Invoke($"Verify failed: {www.error}");
            }
        }
    }

    public void Logout()
    {
        ClearToken();
        OnLogout?.Invoke();

#if UNITY_WEBGL && !UNITY_EDITOR
        LogoutFromBrowser();
#endif
    }

    private void ClearToken()
    {
        AccessToken = null;
        CurrentUser = null;
    }



    private class VerifyResponse
    {
        [JsonProperty("valid")]
        public bool Valid { get; set; }

        [JsonProperty("user")]
        public UserData User { get; set; }
    }

    // API Helper

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

            if (www.result == UnityWebRequest.Result.Success)
                onSuccess?.Invoke(www.downloadHandler.text);
            else
                onError?.Invoke(www.error);
        }
    }

    public IEnumerator Post(
        string endpoint,
        object body,
        Action<string> onSuccess,
        Action<string> onError = null)
    {
        if (!IsLoggedIn) { onError?.Invoke("Not logged in"); yield break; }

        string json = JsonConvert.SerializeObject(body);
        byte[] raw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest www = new UnityWebRequest($"{BACKEND_URL}{endpoint}", "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(raw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", $"Bearer {AccessToken}");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
                onSuccess?.Invoke(www.downloadHandler.text);
            else
                onError?.Invoke(www.error);
        }
    }
}
