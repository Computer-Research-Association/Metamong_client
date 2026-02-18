using UnityEngine;

/// <summary>
/// WebGL JavaScript에서 호출될 콜백을 받는 MonoBehaviour
/// OAuthManager가 자동으로 생성
/// </summary>
public class OAuthCallbackReceiver : MonoBehaviour
{
    private OAuthManager oauthManager;

    public void Initialize(OAuthManager manager)
    {
        oauthManager = manager;
    }

    // JavaScript에서 호출되는 메서드
    public void OnOAuthSuccess(string token)
    {
        Debug.Log("[Callback Receiver] Success received from JavaScript");
        if (oauthManager != null)
        {
            oauthManager.OnOAuthSuccess(token);
        }
    }

    public void OnOAuthError(string error)
    {
        Debug.Log("[Callback Receiver] Error received from JavaScript");
        if (oauthManager != null)
        {
            oauthManager.OnOAuthError(error);
        }
    }
}