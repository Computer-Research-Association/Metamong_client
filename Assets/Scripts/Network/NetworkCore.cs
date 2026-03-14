using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections;
using Metamong.Core;

public class NetworkCore : INetworkProvider
{
    public static NetworkCore Instance { get; private set; } = new NetworkCore();

    private ColyseusHandler _colyseusHandler;
    private FastAPIHandler _fastAPIHandler;

    private NetworkSetting _settings;

    public event Action<Vector2> OnServerPositionReceived;
    public void Initialize(NetworkSetting settings, MonoBehaviour coroutineRunner)
    {
        //Instance = this;
        _settings = settings;
        _colyseusHandler = new ColyseusHandler(settings.colyseusServerUrl); // 또는 주입받음
        _fastAPIHandler = new FastAPIHandler(coroutineRunner, settings.fastApiBaseUrl);
    }
    //private readonly FastAPIHandler _fastApi;


    public void SendMove(Vector2 direction)
    {
        _colyseusHandler.SendMove(direction);
    }

    //수정해야함, 범용성이 너무 낮지만 일단 씀,,
    public async Task JoinSquare()
    {
        await _colyseusHandler.JoinRoom<MyRoomState>(_settings.gameRoomName, _settings.jwt);
    }

    public void SubscribeLocalData(Action<Vector2> e) => _colyseusHandler.onPositionReceived += e;
    public void SubscribeRemoteData(Action<string, Vector2> e) => _colyseusHandler.OnPlayerUpdateReceived += e;

    //----------------------------------------------------------------------------------------------------------------------------

    public void ReceiveToken(string token)
    {
        _fastAPIHandler.ReceiveToken(token);
    }

    public IEnumerator InitializeUser(
        InitializeUserRequest initData,
        Action<UserData> onSuccess = null,
        Action<string> onError = null)
    {
        return _fastAPIHandler.InitializeUser(initData, onSuccess, onError);
    }


    public IEnumerator UpdateRC(
        RC newRc,
        Action<UserData> onSuccess = null,
        Action<string> onError = null)
    {
        return _fastAPIHandler.UpdateRC(newRc, onSuccess, onError);
    }

    public void Logout()
    {
        _fastAPIHandler.Logout();
    }

    public void LoadData(UserData_ userData)
    {
        //API 호출을 통해 받은 userData를 userData에 주입하는 FastAPI 함수를 실행해야함
        //이건 테스트
        foreach(AvatarData avatarData in _settings.TestUserData.Avatars.GetAvatars())
        {           
            userData.Avatars.AddAvatar(avatarData);
        }
    }

    
}
