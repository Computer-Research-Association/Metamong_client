using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers _instance;
    public static Managers Instance { get { Init(); return _instance; } }

    #region [Sub Managers]

    // Non-MonoBehaviour Managers (지연 초기화)
    ResourceManager _resource;
    UIManager _ui;
    AuthManager _auth;
    UserManager _user;
    OAuthManager _oauth;

    public static ResourceManager Resource { get { return Instance._resource; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static AuthManager Auth { get { return Instance._auth; } }
    public static UserManager User { get { return Instance._user; } }
    public static OAuthManager OAuth { get { return Instance._oauth; } }

    #endregion

    void Awake()
    {
        Init();
        InitializeManagers();
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

    private void InitializeManagers()
    {
        // Awake에서 Manager 인스턴스 생성
        if (_resource == null) _resource = new ResourceManager();
        if (_ui == null) _ui = new UIManager();
        if (_auth == null) _auth = new AuthManager();
        if (_user == null) _user = new UserManager();
        if (_oauth == null) _oauth = new OAuthManager();

        Debug.Log("[Managers] All managers initialized");
    }
}