using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Metamong.Core;

/// <summary>
/// Non-MonoBehaviour OAuth Manager
/// Editor: Mock 토큰 사용
/// WebGL Build: 실제 OAuth 플로우
/// </summary>
public class OAuthManager
{
    private string API_BASE_URL => AppConfig.Instance.apiBaseUrl;
    private CoroutineRunner coroutineRunner;

    // Editor 테스트용 Mock 토큰
    [Header("Editor Test Settings")]
    public string mockToken = ""; // Inspector에서 실제 토큰 붙여넣기

    // 이벤트
    public event Action<UserData> OnUserLoggedIn;

    public OAuthManager()
    {
        // Coroutine 실행을 위한 MonoBehaviour 생성
        GameObject go = new GameObject("OAuthCoroutineRunner");
        UnityEngine.Object.DontDestroyOnLoad(go);
        coroutineRunner = go.AddComponent<CoroutineRunner>();

#if UNITY_WEBGL && !UNITY_EDITOR
        // WebGL 빌드에서만 콜백 리시버 생성
        GameObject receiver = new GameObject("OAuthCallbackReceiver");
        UnityEngine.Object.DontDestroyOnLoad(receiver);
        var receiverScript = receiver.AddComponent<OAuthCallbackReceiver>();
        receiverScript.Initialize(this);
        Debug.Log("[OAuthManager] WebGL callback receiver initialized");
#endif

        Debug.Log("[OAuthManager] Initialized");
    }

    #region [OAuth Login Flow]

    public void LoginWithGoogle()
    {
        StartOAuthFlow("google");
    }

    public void LoginWithKakao()
    {
        StartOAuthFlow("kakao");
    }

    public void LoginWithNaver()
    {
        StartOAuthFlow("naver");
    }

    private void StartOAuthFlow(string provider)
    {
        Debug.Log($"[OAuth] Starting login with {provider}");

#if UNITY_EDITOR
        // Unity Editor에서는 Mock 토큰 사용
        if (string.IsNullOrEmpty(mockToken))
        {
            Debug.LogError("[OAuth] Mock token is empty! Please set a valid token in the Inspector.");
            Debug.LogError("[OAuth] To get a token: Login via WebGL build, then copy token from Console.");
            return;
        }

        Debug.Log($"[OAuth Editor] Using mock token for {provider}");
        coroutineRunner.StartCoroutine(SimulateOAuthSuccess());
        return;
#else
        // WebGL 빌드에서는 실제 OAuth 플로우
        string loginURL = $"{API_BASE_URL}/auth/login/{provider}";
        Debug.Log($"[OAuth] Login URL: {loginURL}");

#if UNITY_WEBGL
        // WebGL에서는 JavaScript 함수 호출
        Application.ExternalCall("OpenOAuthWindow", loginURL);
#else
        // 다른 플랫폼 (Standalone 등)
        Application.OpenURL(loginURL);
        Debug.LogWarning("[OAuth] Browser opened. OAuth callback may not work on this platform.");
#endif
#endif
    }

    private IEnumerator SimulateOAuthSuccess()
    {
        yield return new WaitForSeconds(0.5f); // 짧은 딜레이
        Debug.Log("[OAuth Editor] Simulating OAuth success with mock token");
        OnOAuthSuccess(mockToken);
    }

    #endregion

    #region [OAuth Callbacks]

    public void OnOAuthSuccess(string token)
    {
        Debug.Log("[OAuth] Login successful!");
        Debug.Log($"[OAuth] Token received (length: {token.Length})");

        // AuthManager에 토큰 저장
        Managers.Auth.SetToken(token);

        // 유저 정보 가져오기
        coroutineRunner.StartCoroutine(GetMe(
            onSuccess: (userData) =>
            {
                Debug.Log($"[OAuth] ✓ Login complete! Welcome, {userData.Nickname}!");

                // UserManager에 저장
                Managers.User.SetUser(userData);
                Managers.User.SaveUserDataLocally();

                // 이벤트 발생
                OnUserLoggedIn?.Invoke(userData);
            },
            onError: (error) =>
            {
                Debug.LogError($"[OAuth] Failed to get user info: {error}");
            }
        ));
    }

    public void OnOAuthError(string error)
    {
        Debug.LogError($"[OAuth] Login failed: {error}");
    }

    #endregion

    #region [API Requests]

    public IEnumerator GetMe(Action<UserData> onSuccess, Action<string> onError)
    {
        string url = $"{API_BASE_URL}/users/me";
        Debug.Log($"[OAuth] Fetching user info from: {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Authorization 헤더
            string authHeader = Managers.Auth.GetAuthorizationHeader();

            if (string.IsNullOrEmpty(authHeader))
            {
                Debug.LogError("[OAuth] No valid token available");
                onError?.Invoke("No valid token");
                yield break;
            }

            request.SetRequestHeader("Authorization", authHeader);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"[OAuth] API Response received (length: {jsonResponse.Length})");

                try
                {
                    UserData userData = JsonConvert.DeserializeObject<UserData>(jsonResponse);

                    if (userData != null)
                    {
                        Debug.Log($"[OAuth] User parsed: {userData.Nickname} (ID: {userData.Id})");
                        Debug.Log($"[OAuth] - Email: {userData.Email}");
                        Debug.Log($"[OAuth] - Provider: {userData.AuthProvider}");
                        Debug.Log($"[OAuth] - RC: {userData.Rc}");
                        Debug.Log($"[OAuth] - Status: {userData.Status}");
                        onSuccess?.Invoke(userData);
                    }
                    else
                    {
                        Debug.LogError("[OAuth] Deserialized user data is null");
                        onError?.Invoke("Invalid user data");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[OAuth] JSON Parsing Error: {ex.Message}");
                    Debug.LogError($"[OAuth] Response was: {jsonResponse}");
                    onError?.Invoke($"JSON parsing failed: {ex.Message}");
                }
            }
            else
            {
                Debug.LogError($"[OAuth] Request failed: {request.error}");
                Debug.LogError($"[OAuth] Response Code: {request.responseCode}");

                if (request.responseCode == 401)
                {
                    Debug.LogWarning("[OAuth] Token expired or invalid. Clearing token.");
                    Managers.Auth.ClearToken();
                    onError?.Invoke("Token expired. Please login again.");
                }
                else
                {
                    onError?.Invoke(request.error);
                }
            }
        }
    }

    #endregion

    #region [Public Methods]

    public void Logout()
    {
        Debug.Log("[OAuth] Logging out...");
        Managers.Auth.ClearToken();
        Managers.User.ClearUser();
        Managers.User.ClearLocalUserData();
    }

    public void RefreshUserInfo(Action<UserData> onSuccess, Action<string> onError)
    {
        if (!Managers.Auth.IsLoggedIn)
        {
            Debug.LogWarning("[OAuth] Not logged in");
            onError?.Invoke("Not logged in");
            return;
        }

        coroutineRunner.StartCoroutine(GetMe(onSuccess, onError));
    }

    #endregion

    // Coroutine 실행을 위한 헬퍼 MonoBehaviour
    private class CoroutineRunner : MonoBehaviour { }
}