using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Editor에서 Mock 토큰을 쉽게 설정하기 위한 유틸리티
/// </summary>
public class MockTokenHelper : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Mock Token Settings")]
    [TextArea(3, 10)]
    [Tooltip("WebGL 빌드에서 받은 실제 토큰을 여기에 붙여넣으세요")]
    public string mockToken = "";

    [Header("Quick Actions")]
    [Tooltip("Inspector에서 버튼으로 사용")]
    public bool applyToken;

    [Tooltip("토큰 초기화")]
    public bool clearToken;

    private void OnValidate()
    {
        if (applyToken)
        {
            applyToken = false;
            ApplyMockToken();
        }

        if (clearToken)
        {
            clearToken = false;
            ClearMockToken();
        }
    }

    private void ApplyMockToken()
    {
        if (string.IsNullOrEmpty(mockToken))
        {
            Debug.LogWarning("[Mock Token] Token is empty!");
            return;
        }

        Managers.OAuth.mockToken = mockToken;
        Debug.Log($"[Mock Token] Applied token (length: {mockToken.Length})");
        Debug.Log("[Mock Token] You can now test login in Play mode");
    }

    private void ClearMockToken()
    {
        mockToken = "";
        Managers.OAuth.mockToken = "";
        Managers.Auth.ClearToken();
        Debug.Log("[Mock Token] Cleared");
    }

    [ContextMenu("Test Login with Mock Token")]
    private void TestLogin()
    {
        if (string.IsNullOrEmpty(mockToken))
        {
            Debug.LogError("[Mock Token] Please set a token first!");
            return;
        }

        ApplyMockToken();
        Managers.OAuth.LoginWithGoogle();
    }
#endif
}