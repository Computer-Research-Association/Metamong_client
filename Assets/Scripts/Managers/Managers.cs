using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers _instance;
    public static Managers Instance { get { Init(); return _instance; } }

    #region [Sub Managers]

    AuthManager _auth = new AuthManager();
    OAuthManager _oauth = new OAuthManager();
    ResourceManager _resource = new ResourceManager();
    UIManager _ui = new UIManager();
    UserManager _user = new UserManager();


    public static AuthManager Auth { get { return Instance._auth; } }
    public static OAuthManager OAuth { get { return Instance._oauth; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static UserManager User { get { return Instance._user; } }


    #endregion

    void Awake()
    {
        Init();
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
