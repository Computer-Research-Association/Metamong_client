using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class AuthClient : MonoBehaviour
{

    private AppConfig config;
    public string AccessToken { get; private set; }

    public void Init(AppConfig appConfig)
    {
        config = appConfig;
        Debug.Log("[AuthClient] Config Loaded");
    }

    public void LoginWithGoogle()
    {
        if (config == null)
        {
            Debug.LogError("AppConfig is not initialized in AuthClient!");
            return;
        }

        string loginUrl = $"{config.apiBaseUrl}/auth/login/google";

        Debug.Log($"[Auth] Opening Login URL: {loginUrl}");
        Application.OpenURL(loginUrl);
    }

    public void SetToken(string token)
    {
        AccessToken = token;
        PlayerPrefs.SetString("JWT_TOKEN", token);
        Debug.Log($"[Auth] Token Saved: {token}");
    }

    // TODO: JsonConverter 와 스키마 적용해야 함.
    public IEnumerator GetMe(Action<string> successCallback, Action<string> errorCallback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get($"{config.apiBaseUrl}/users/me"))
        {
            webRequest.SetRequestHeader("Authorization", $"Bearer {AccessToken}");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                successCallback?.Invoke(webRequest.downloadHandler.text);
            }
            else
            {
                errorCallback?.Invoke(webRequest.error);
            }
        }
    }

}
