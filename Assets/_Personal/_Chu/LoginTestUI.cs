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
            logText.text = "유니티 로딩 완료. 웹 토큰 대기 중...";
        }

        // Managers.Auth 이벤트 구독
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
            logText.text = $"<color=#00FF00>로그인 성공!</color>\n\n" +
                           $"닉네임: {user.Nickname}\n" +
                           $"플랫폼: {user.AuthProvider}\n" +
                           $"RC: {user.Rc}\n" +
                           $"상태: {user.Status}";
        }
    }

    private void HandleLoginFail(string reason)
    {
        if (logText != null)
        {
            logText.text = $"<color=#FF0000>로그인 실패</color>\n사유: {reason}";
        }
    }
}