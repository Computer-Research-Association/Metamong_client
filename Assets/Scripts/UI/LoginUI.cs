using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Metamong.Core;
using System.Data;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{
    [Header("Google Login")]
    [SerializeField] private Button loginButton;

    [Header("Test: Manual Token Login")]
    [SerializeField] private TMP_InputField tokenInputField;
    [SerializeField] private Button tokenLoginButton;
    [SerializeField] private TextMeshProUGUI statusText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnGoogleLoginClick);
        }

        if (tokenLoginButton != null)
        {
            tokenLoginButton.onClick.AddListener(OnTokenLoginClick);
        }
    }

    public void OnGoogleLoginClick()
    {
        Debug.Log("Google login clicked");

        if (NetworkManager.Instance != null && NetworkManager.Instance.Auth != null)
        {
            NetworkManager.Instance.Auth.LoginWithGoogle();
        }
        else
        {
            Debug.LogError("NetworkManager or AuthClient is not initialized.");
        }
    }

    public void OnTokenLoginClick()
    {
        string token = tokenInputField.text;

        if (string.IsNullOrEmpty(token))
        {
            UpdateStatus("Enter Token!!!!!", Color.red);
            return;
        }

        if (IsNetworkReady())
        {
            UpdateStatus("Finding user from server...", Color.yellow);

            // AuthClient에 토큰 설정
            NetworkManager.Instance.Auth.SetToken(token);

            // 내 정보 가져오기 (성공/실패 콜백 전달)
            StartCoroutine(NetworkManager.Instance.Auth.GetMe(OnLoginSuccess, OnLoginFail));
        }
    }


    private void OnLoginSuccess(UserData user)
    {
        UpdateStatus($"Login Success!\nWelcome, {user.Nickname}님 ({user.Rc})", Color.green);
        Debug.Log($"[LoginUI] User: {user.Nickname}, RC: {user.Rc}");

        NetworkManager.Instance.SetCurrentUser(user);

        Invoke(nameof(LoadGameScene), 1.0f); // 1초 뒤 광장 씬 이동
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene("MainSquareTest");
    }

    private void OnLoginFail(string error)
    {
        UpdateStatus($"Login Fail: {error}", Color.red);
    }

    private void UpdateStatus(string msg, Color color)
    {
        if (statusText != null)
        {
            statusText.text = msg;
            statusText.color = color;
        }
    }


    private bool IsNetworkReady()
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.Auth != null)
        {
            return true;
        }
        // 에러 로그 출력
        Debug.LogError("NetworkManager or AuthClient is not initialized.");
        UpdateStatus("NetworkManager Error", Color.red);
        return false;
    }
}
