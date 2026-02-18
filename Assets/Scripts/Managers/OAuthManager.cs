using System.Collections;
using System.Runtime.InteropServices;
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
            return;
        }
#endif

        string loginURL = $"{API_BASE_URL}/auth/login/{provider}";


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
    }

    public void OnOAuthError(string error)
    {
        Debug.LogError("OAuth Error: " + error);
    }

    private IEnumerator GetUserInfo(string token)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{API_BASE_URL}/api/user/me"))
        {
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("User Info: " + request.downloadHandler.text);
                // TODO: 유저 정보 처리
            }
            else
            {
                Debug.LogError("Failed to get user info: " + request.error);
            }
        }
    }

}
