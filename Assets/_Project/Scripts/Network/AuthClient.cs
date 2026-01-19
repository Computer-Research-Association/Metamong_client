using UnityEngine;

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

}
