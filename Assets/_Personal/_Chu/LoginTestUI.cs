using UnityEngine;
using TMPro;
using Metamong.Core;

public class LoginTestUI : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI logText; // Canvas의 Text (TMP) 연결

    private void Start()
    {
        if (logText != null)
        {
            logText.text = "Unity Loading Complete. Waiting Web Token...";
        }

        // Managers.Auth 이벤트 구독
        Managers.Auth.OnLoginComplete -= HandleLoginSuccess;
        Managers.Auth.OnLoginFailed -= HandleLoginFail;
        Managers.Auth.OnLoginComplete += HandleLoginSuccess;
        Managers.Auth.OnLoginFailed += HandleLoginFail;
    }

    private void OnDestroy()
    {
        // 씬이 넘어가거나 파괴될 때 구독 해제 (메모리 누수 방지)
        if (Managers.Instance != null)
        {
            Managers.Auth.OnLoginComplete -= HandleLoginSuccess;
            Managers.Auth.OnLoginFailed -= HandleLoginFail;
        }
    }

    private void HandleLoginSuccess(UserData user)
    {
        if (logText != null)
        {
            logText.text = $"<color=#00FF00>Logged In!</color>\n\n" +
                           $"Nickname: {user.Nickname}\n" +
                           $"Platform: {user.AuthProvider}\n" +
                           $"RC: {user.Rc}\n" +
                           $"UserStatus: {user.Status}";
        }
    }

    private void HandleLoginFail(string reason)
    {
        if (logText != null)
        {
            logText.text = $"<color=#FF0000>Login Failed</color>\nReason: {reason}";
        }
    }
}