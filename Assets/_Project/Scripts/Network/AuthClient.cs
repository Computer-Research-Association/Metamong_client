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



}
