using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class NetworkCore : INetworkProvider
{
    public static NetworkCore Instance { get; private set; } = new NetworkCore();

    public ColyseusHandler Handler { get; private set; }

    private NetworkSetting _settings;

    public event Action<Vector2> OnServerPositionReceived;
    public void Initialize(NetworkSetting settings)
    {
        //Instance = this;
        _settings = settings;
        Handler = new ColyseusHandler(settings.colyseusServerUrl); // 또는 주입받음
    }
    //private readonly FastAPIHandler _fastApi;


    public void SendMove(Vector2 direction)
    {
        Handler.SendMove(direction);
    }

    //수정해야함, 범용성이 너무 낮지만 일단 씀,,
    public async Task JoinSquare()
    {
        await Handler.JoinRoom<MyRoomState>(_settings.gameRoomName, _settings.jwt);
    }

    public void SubscribeLocalData(Action<Vector2> e) => Handler.onPositionReceived += e;
    public void SubscribeRemoteData(Action<string, Vector2> e) => Handler.OnPlayerUpdateReceived += e;

}
