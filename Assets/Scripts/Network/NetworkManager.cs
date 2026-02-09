using Metamong.Core;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    [SerializeField] private AppConfig config;

    public AuthClient Auth { get; private set; }

    public UserData CurrentUser { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitClients();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitClients()
    {
        Auth = gameObject.AddComponent<AuthClient>();
        Auth.Init(config);
    }

    public void SetCurrentUser(UserData user)
    {
        CurrentUser = user;
        Debug.Log($"[NetworkManager] Logged In User Saved: {user.Nickname} (RC: {user.Rc})");
    }
}
