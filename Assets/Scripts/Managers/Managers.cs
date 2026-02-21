using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers _instance;
    public static Managers Instance { get { Init(); return _instance; } }

    #region [Sub Managers]

    AuthManager _auth = new AuthManager();
    ResourceManager _resource = new ResourceManager();
    UIManager _ui = new UIManager();


    public static AuthManager Auth { get { return Instance._auth; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static UIManager UI { get { return Instance._ui; } }

    #endregion

    void Awake()
    {
        Init();
    }

    void Start()
    {
        // AuthManager 에 Unity 준비 완료 신호 전달
        // -> jslib 통해 index.html 의 onUnityReady() 호출 -> SendMessage로 토큰 들어옴
        _auth.Init();
    }

#if UNITY_EDITOR
    [Header("Debug Settings")]
    [Tooltip("에디터에서 테스트할 때 사용할 JWT 토큰을 입력하세요.")]
    [SerializeField] private string _testAccessToken;

    [ContextMenu("Debug/Force Login with Test Token")]
    public void ForceLogin()
    {
        if (string.IsNullOrEmpty(_testAccessToken))
        {
            Debug.LogWarning("테스트 토큰이 비어 있습니다! 인스펙터에서 토큰을 입력해 주세요.");
            return;
        }

        Debug.Log("[Debug] 인스펙터 토큰으로 강제 로그인 시도");
        OnReceiveAuthToken(_testAccessToken);
    }
#endif

    public void OnReceiveAuthToken(string token)
    {
        Debug.Log("[Managers] Auth token received from browser");
        _auth.ReceiveToken(token);
    }

    private static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            _instance = go.GetComponent<Managers>();
        }
    }
}
