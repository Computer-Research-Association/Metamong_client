using System.Threading.Tasks;
using UnityEngine;

public class NetworkRunner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private NetworkCore _networkCore;
    public NetworkSetting settings;
    void Awake()
    {
        NetworkCore.Instance.Initialize(settings);
    }

    async Task Start()
    {
        await NetworkCore.Instance.JoinSquare();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
