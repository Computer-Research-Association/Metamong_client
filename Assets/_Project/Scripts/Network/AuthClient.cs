using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using Metamong.Core;
using Newtonsoft.Json;

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

    public IEnumerator GetMe(Action<UserData> successCallback, Action<string> errorCallback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get($"{config.apiBaseUrl}/users/me"))
        {
            webRequest.SetRequestHeader("Authorization", $"Bearer {AccessToken}");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;

                try
                {
                    // Newtonsoft.Json을 사용하여 역직렬화
                    UserData user = JsonConvert.DeserializeObject<UserData>(jsonResponse);
                    Debug.Log($"[Auth] Welcome, {user.Nickname}!");
                    successCallback?.Invoke(user);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[Auth] JSON Parsing Error: {ex.Message}");
                    errorCallback?.Invoke(ex.Message);
                }
            }
            else
            {
                errorCallback?.Invoke(webRequest.error);
            }
        }
    }

}
