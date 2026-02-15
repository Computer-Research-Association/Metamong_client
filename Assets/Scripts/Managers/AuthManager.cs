using UnityEngine;

public class AuthManager
{
    public string AccessToken { get; private set; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);
}
