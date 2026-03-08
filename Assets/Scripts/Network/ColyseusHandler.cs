using UnityEngine;
using Colyseus;
using Colyseus.Schema;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using NUnit.Framework;
using System.Collections.Generic;

public class ColyseusHandler
{
    private ColyseusClient _client;
    //private ColyseusRoom<Schema> _room;
    private ColyseusRoom<MyRoomState> _room;
    // Core로부터 전달받은 "보고용" 액션 변수
    public Action<Vector2> onPositionReceived;
    public Action<string, Vector2> OnPlayerUpdateReceived;
    public ColyseusHandler(string url)
    {
        _client = new ColyseusClient(url);
    }

    public void SendMove(Vector2 dir)
    {
        if (_room == null) return;
        Debug.Log("check dir vector: " + dir);
        _room.Send("move", new { x = dir.x, y = dir.y });
    }

    public async Task<bool> JoinRoom<T>(string roomName, string token) where T : Schema
    {
        try
        {
            //var room = await _client.JoinOrCreate<T>(roomName);
            //_room = room as ColyseusRoom<Schema>;
            var options = new Dictionary<string, object>
            {
                { "token", token }
            };
            _room = await _client.JoinOrCreate<MyRoomState>(roomName, options);
            Debug.Log($"[Colyseus] Joined room: {_room.SessionId}");

            //BindMessages();
            BindStateEvents();
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Colyseus] Join failed: {e.Message}");
            return false;
        }
    }

    private void BindMessages()
    {
        if (_room == null) return;

        // 서버에서 "server_pos" 메시지를 보낼 때 처리
        _room.OnMessage<Vector2Serialized>("server_pos", (data) =>
        {
            onPositionReceived?.Invoke(new Vector2(data.x, data.y));
        });
    }


    //remove 미완성
    //todo: remove, remotePlayerManager, NetworkRunner
    private void BindStateEvents()
    {
        if (_room == null) return;
        _room.OnStateChange += (state, isFirstState) =>
        {
            state.players.ForEach((key, player) =>
            {
                Debug.Log($"플레이어 ID: {key}, 위치: {player.x}, {player.y}");
                if (key == _room.SessionId)
                {
                    onPositionReceived?.Invoke(new Vector2(player.x, player.y));
                }
                else
                {
                    //RemotePlayerManager는 가지고 있는 Dictionary에서 유저가 있으면 move를 실행, 없으면 spawn을 실행
                   OnPlayerUpdateReceived?.Invoke(key, new Vector2(player.x, player.y));
                }
            });
        };
    }

}
