using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    [SerializeField] private AppConfig config;

    public AuthClient Auth { get; private set; }

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
}
