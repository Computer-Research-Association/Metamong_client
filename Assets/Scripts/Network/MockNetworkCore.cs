using System;
using UnityEngine;

public class MockNetworkCore : INetworkProvider
{
    public static MockNetworkCore Instance { get; private set; } = new MockNetworkCore();

    event Action<Vector2> INetworkProvider.OnServerPositionReceived
    {
        add
        {
            throw new NotImplementedException();
        }

        remove
        {
            throw new NotImplementedException();
        }
    }

    public void SendMove(Vector2 direction)
    {
        
    }
    // 서버로부터 위치를 받았을 때 실행할 액션
}
