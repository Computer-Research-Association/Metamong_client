using UnityEngine;
using UnityEngine.UI;
using Metamong.Core;

public class SimpleLoginUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button googleLoginButton;
    [SerializeField] private Button kakaoLoginButton;
    [SerializeField] private Button naverLoginButton;
    [SerializeField] private Text statusText;

    private void Start()
    {
        // 버튼 이벤트 연결
        if (googleLoginButton != null)
            googleLoginButton.onClick.AddListener(() => OnLoginClicked("google"));

        if (kakaoLoginButton != null)
            kakaoLoginButton.onClick.AddListener(() => OnLoginClicked("kakao"));

        if (naverLoginButton != null)
            naverLoginButton.onClick.AddListener(() => OnLoginClicked("naver"));

        // OAuthManager 이벤트 구독
        Managers.OAuth.OnUserLoggedIn += OnUserLoggedIn;

        UpdateStatus("Ready to login");
        Debug.Log("[Login UI] Initialized");

        // 이미 로그인되어 있는지 확인
        if (Managers.Auth.IsLoggedIn)
        {
            Debug.Log("[Login UI] Already logged in. Checking token...");
            UpdateStatus("Checking existing login...");
            CheckExistingLogin();
        }
    }

    private void OnDestroy()
    {
        Managers.OAuth.OnUserLoggedIn -= OnUserLoggedIn;
    }

    private void OnLoginClicked(string provider)
    {
        Debug.Log($"[Login UI] {provider} login button clicked");
        UpdateStatus($"Starting {provider} login...");

        switch (provider)
        {
            case "google":
                Managers.OAuth.LoginWithGoogle();
                break;
            case "kakao":
                Managers.OAuth.LoginWithKakao();
                break;
            case "naver":
                Managers.OAuth.LoginWithNaver();
                break;
        }
    }

    private void CheckExistingLogin()
    {
        Managers.OAuth.RefreshUserInfo(
            onSuccess: (userData) =>
            {
                UpdateStatus($"Welcome back, {userData.Nickname}!");
                Debug.Log($"[Login UI] Token is valid. User: {userData.Nickname}");
            },
            onError: (error) =>
            {
                UpdateStatus("Please login");
                Debug.Log($"[Login UI] Token expired or invalid: {error}");
            }
        );
    }

    private void OnUserLoggedIn(UserData userData)
    {
        Debug.Log($"[Login UI] ===== LOGIN SUCCESS =====");
        Debug.Log($"[Login UI] Nickname: {userData.Nickname}");
        Debug.Log($"[Login UI] Email: {userData.Email}");
        Debug.Log($"[Login UI] Provider: {userData.AuthProvider}");
        Debug.Log($"[Login UI] RC: {userData.Rc}");
        Debug.Log($"[Login UI] Status: {userData.Status}");
        Debug.Log($"[Login UI] ==========================");

        UpdateStatus($"✓ Welcome, {userData.Nickname}!\nLogin successful!");

        // 3초 후 다음 Scene으로 이동 (옵션)
        // Invoke("LoadMainScene", 3f);
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        Debug.Log($"[Login UI] Status: {message}");
    }

    private void LoadMainScene()
    {
        // TODO: 메인 씬으로 전환
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }
}