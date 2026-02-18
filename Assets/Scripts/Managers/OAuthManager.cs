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

    public void OnOAuthSuccess(string token)
    {
        Debug.Log($"OAuth Success! Token: " + token);

        // AuthManager에 토큰 저장
        Managers.Auth.SetToken(token);

        // TODO: 유저 정보 가져오기
    }

    public void OnOAuthError(string error)
    {
        Debug.LogError("OAuth Error: " + error);
    }


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
}
