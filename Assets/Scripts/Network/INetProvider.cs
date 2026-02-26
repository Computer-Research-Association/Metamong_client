using UnityEngine;
using System;
public interface INetworkProvider
{
    public void SendMove(Vector2 direction);
    // 서버로부터 위치를 받았을 때 실행할 액션
    event Action<Vector2> OnServerPositionReceived; 
}