using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Metamong.Core;

/// <summary>
/// 테스트 씬 1 전용 Welcome UI (UGUI).
/// Canvas 하위에 WelcomeText(TMP), EnterButton을 배치하고 이 컴포넌트를 연결한다.
/// </summary>
public class TestWelcomeUI : MonoBehaviour
{
    [Header("씬 2 이름 (Build Settings에 등록된 씬 이름)")]
    [SerializeField] private string _gameSceneName = "GameScene";

    [Header("UGUI References")]
    [SerializeField] private TMP_Text _welcomeText;
    [SerializeField] private Button _enterButton;

    void Start()
    {
        _enterButton.gameObject.SetActive(false);
        _enterButton.onClick.AddListener(OnEnterClicked);

        // 이미 로그인 완료 상태라면 바로 표시 (에디터 DevLogin이 빠른 경우 대비)
        if (NetworkCore.Instance.IsLoggedIn)
        {
            ShowWelcome(NetworkCore.Instance.CurrentUser);
            return;
        }

        NetworkCore.Instance.OnLoginComplete += ShowWelcome;
    }

    void OnDestroy()
    {
        NetworkCore.Instance.OnLoginComplete -= ShowWelcome;
    }

    private void ShowWelcome(UserData user)
    {
        _welcomeText.text = $"환영합니다, {user.Nickname}님!";
        _enterButton.gameObject.SetActive(true);
    }

    private void OnEnterClicked()
    {
        SceneManager.LoadScene(_gameSceneName);
    }
}
