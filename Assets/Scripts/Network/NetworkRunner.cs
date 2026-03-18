using UnityEngine;

public class NetworkRunner : MonoBehaviour
{
    public NetworkSetting settings;

    void Awake()
    {
        // 씬 전환 시 중복 생성 방지
        if (FindObjectsByType<NetworkRunner>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        NetworkCore.Instance.Initialize(settings, this);
    }
}
