using UnityEngine;

public class AuthManager
{
    public string AccessToken { get; private set; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);

    public void OnLoginSuccess(string token)
    {
        AccessToken = token;
    }

    public void RequestLogin(string provider)
    {
#if UNITY_WEBGL

        

#endif
    }
}
