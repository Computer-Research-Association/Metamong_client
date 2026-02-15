using UnityEngine;
using Metamong.Core;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("2D Settings")]
    [SerializeField] private GameObject playerPrefab; // 2D Sprite 프리팹
    [SerializeField] private Transform spawnPoint;    // 시작 위치

    // RC별 색상 매핑
    // TODO: 우선은 아무 색상이나 대입했으므로 차후 에셋 변경에 따라 색상은 지우거나 수정이 필요함.
    private Dictionary<RC, Color> rcColors = new Dictionary<RC, Color>()
    {
        { RC.Torrey, Color.red },
        { RC.JangGiRyeo, Color.green },
        { RC.Carmichael, Color.blue },
        { RC.Kuyper, Color.yellow },
        { RC.SonYangWon, Color.cyan },
        { RC.Philadelphos, new Color(1f, 0f, 1f) } // Magenta
    };

    void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        SpawnLocalPlayer();
    }

    private void SpawnLocalPlayer()
    {
        // 1. 유저 데이터 가져오기
        if (NetworkManager.Instance == null || NetworkManager.Instance.CurrentUser == null)
        {
            Debug.LogWarning("테스트 모드: Torrey팀 캐릭터 생성");
            Create2DCharacter("TestPlayer", RC.Torrey);
            return;
        }

        UserData user = NetworkManager.Instance.CurrentUser;
        Create2DCharacter(user.Nickname, user.Rc);
    }

    private void Create2DCharacter(string nickname, RC rc)
    {
        // 2D 평면이므로 Z값은 0으로 고정
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        spawnPos.z = 0;

        GameObject myPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        myPlayer.name = $"LocalPlayer_{nickname}";

        // [변경점] SpriteRenderer의 색상을 변경
        SpriteRenderer sr = myPlayer.GetComponent<SpriteRenderer>();
        if (sr != null && rcColors.TryGetValue(rc, out Color teamColor))
        {
            sr.color = teamColor;
        }

        Debug.Log($"[PlayerManager] 2D 캐릭터 소환 완료: {nickname} ({rc})");

        // TODO: 캐릭터 머리 위에 닉네임 텍스트(Canvas World Space) 추가 필요
    }

}
