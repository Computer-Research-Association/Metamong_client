using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private Button loginButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnGoogleLoginClick);
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
            Debug.LogError("NetworkManager 또는 AuthClient가 초기화되지 않았습니다.");
        }
    }
}
