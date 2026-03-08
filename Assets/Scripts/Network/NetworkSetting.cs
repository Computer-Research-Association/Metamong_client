using UnityEngine;

[CreateAssetMenu(fileName = "NetworkSetting", menuName = "Network/NetworkSetting")]
public class NetworkSetting : ScriptableObject
{
    [Header("Server Endpoints")]
    public string fastApiBaseUrl = "http://localhost:8000";
    public string colyseusServerUrl = "ws://localhost:2567";
    public string jwt = "";

    [Header("Retry & Timeout")]
    [Range(1, 30)] public int requestTimeout = 10;
    [Range(0, 10)] public int maxRetryCount = 3;

    [Header("Real-time Settings")]
    public string gameRoomName = "my_game_room";
    public bool useSecureConnection = false; // https/wss 사용 여부

    [Header("Debug Options")]
    public bool showNetworkLogs = true;
    public int simulatedLatencyMs = 100; // 네트워크 랙 시뮬레이션
}
