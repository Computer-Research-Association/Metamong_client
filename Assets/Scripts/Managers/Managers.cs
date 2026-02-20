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
