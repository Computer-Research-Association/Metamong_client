using UnityEngine;

public class AuthManager
{
    // jslib import
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void NotifyUnityReady();
    
    [DllImport("__Internal")]
    private static extern void LogoutFromBrowser();
    
    [DllImport("__Internal")]
    private static extern string GetStoredToken();
#endif
}
