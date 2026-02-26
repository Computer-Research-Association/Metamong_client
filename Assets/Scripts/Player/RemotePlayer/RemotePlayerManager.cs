using System.Collections.Generic;
using UnityEngine;

public class RemotePlayerManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created 
    //private Dictionary<string, GameObject> _remotePlayers;
    [SerializeField] private GameObject remotePlayerPrefab; // 인스펙터에서 프리팹 할당
    private Dictionary<string, RemotePlayerController> _remotePlayers; // GameObject 대신 Controller로 관리 추천

    void Awake()
    {
        _remotePlayers = new Dictionary<string, RemotePlayerController>();
        
        if (NetworkCore.Instance != null)
        {
            NetworkCore.Instance.SubscribeRemoteData(SyncRemotePlayer);
        }
    }

    void Start()
    {
        
    }


    void SyncRemotePlayer(string key, Vector2 pos)
    {
        //있으면 GameObject의 position을 변경하도록 Controller에게 전달.
        if (_remotePlayers.TryGetValue(key, out var controller))
        {
            controller.SetTargetPosition(pos);
        }
        else
        {
        // _remotePlayers[key]로 검색했는데 없으면 spawn 실행
            SpawnRemotePlayer(key, pos);
        }
        
    }

    void SpawnRemotePlayer(string key, Vector2 pos)
    {
        //prefab spawn
        GameObject go = Instantiate(remotePlayerPrefab, pos, Quaternion.identity);
        
        //gameobject 추가 _remotePlayers.Add(key,)
        RemotePlayerController controller = go.GetComponent<RemotePlayerController>();   
        controller.Initialize(go.transform.position);
        if (controller != null)
        {
            _remotePlayers.Add(key, controller);
            Debug.Log($"[Remote] Spawned player: {key}");
        }
        else
        {
            Debug.LogError($"{remotePlayerPrefab.name}에 RemotePlayerController가 없습니다!");
        }
    }

}
