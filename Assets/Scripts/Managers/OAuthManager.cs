using System;
using System.Collections;
using System.Runtime.InteropServices;
using Metamong.Core;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class OAuthManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void OpenOAuthWindow(string url);

    private string API_BASE_URL = AppConfig.Instance.apiBaseUrl;

    [Header("Development Only")]
    public bool useMockToken = true;
    public string mockToken = "";


    #region [OAuth Login Flow]

    public void LoginWithGoogle() { StartOAuthFlow("google"); }
    public void LoginWithKakao() { StartOAuthFlow("kakao"); }
    public void LoginWithNaver() { StartOAuthFlow("naver"); }


    private void StartOAuthFlow(string provider)
    {
#if UNITY_EDITOR // Unity Editor에서는 Mock토큰 사용하기
        if (useMockToken)
        {
            Debug.Log("Using Mock Token");
            StartCoroutine(SimulateOAuthSuccess());
            return;
        }
#endif

        string loginURL = $"{API_BASE_URL}/auth/login/{provider}";
        Debug.Log($"[OAuth] Starting login ({provider})");

#if UNITY_WEBGL && !UNITY_EDITOR
        OpenOAuthWindow(loginURL);
#else
        Application.OpenURL(loginURL);
#endif
    }

    private IEnumerator SimulateOAuthSuccess()
    {
        yield return new WaitForSeconds(1f);
        OnOAuthSuccess(mockToken);
    }

    #endregion

    #region [OAuth Callbacks]

    public void OnOAuthSuccess(string token)
    {
        Debug.Log($"OAuth Success! Token: " + token);

        // AuthManager에 토큰 저장
        Managers.Auth.SetToken(token);

        StartCoroutine(GetMe(
            onSuccess: (UserData userData) =>
            {
                Debug.Log($"[OAuth] Welcome, {userData.Nickname}.");
                OnLoginComplete(userData);
            },
            onError: (string error) =>
            {
                Debug.LogError($"[OAuth] Failed to get user info: {error}");
            }
        ));
    }

    public void OnOAuthError(string error)
    {
        Debug.LogError("OAuth Error: " + error);
    }

    #endregion


    public IEnumerator GetMe(Action<UserData> onSuccess, Action<string> onError)
    {
        string url = $"{API_BASE_URL}/users/me";
        Debug.Log($"[OAuth] Fetching user info from: {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            string authHeader = Managers.Auth.GetAuthorizationHeader();
            if (string.IsNullOrEmpty(authHeader))
            {
                Debug.LogError("[OAuth] Invalid token");
                onError?.Invoke("[OAuth] Invalid Token Error");
                yield break;
            }

            request.SetRequestHeader("Authorization", authHeader);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"[OAuth] Response: {jsonResponse}");

                try
                {
                    UserData userData = JsonConvert.DeserializeObject<UserData>(jsonResponse);
                    if (userData != null) // UserData 받아오기 성공
                    {
                        Debug.Log($"[OAUth] User Loaded: {userData}");
                        onSuccess?.Invoke(userData);
                    }
                    else                  // UserData 받아오기 실패
                    {
                        Debug.LogError("[OAuth] UserData Deserialization Failed");
                        onError?.Invoke("Invalid UserData");
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

                // 401 에러면 토큰이 만료된 것
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

    #region [Events]

    public event Action<UserData> OnUserLoggedIn;

    private void OnLoginComplete(UserData userData)
    {
        // Event Invoke
        OnUserLoggedIn?.Invoke(userData);

        // TODO: 이벤트 Invoke 이후 추가 처리 필요시 작업 (Scene 전환, UI 업데이트)
    }

    #endregion

    public void Logout()
    {
        Debug.Log("[OAuth] Logging out");
        Managers.Auth.ClearToken();

        // TODO: Logout event 발생 (Scene 전환 등)
    }

    public void RefreshUserInfo(Action<UserData> onSuccess, Action<string> onError)
    {
        if (!Managers.Auth.IsLoggedIn)
        {
            Debug.LogWarning("[OAuth] Not logged in");
            onError?.Invoke("Not logged in");
            return;
        }

        StartCoroutine(GetMe(onSuccess, onError));
    }


}
