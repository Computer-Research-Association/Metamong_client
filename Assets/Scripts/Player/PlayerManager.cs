using UnityEngine;
using UnityEngine.InputSystem; // 1. 반드시 추가해야 함

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private PlayerBehavior playerBehavior;

    private PlayerDomain _domain;
    private PlayerMoveService _moveService;

    void Awake()
    {
        // 객체 생성 및 의존성 주입
        _domain = new PlayerDomain();
        _moveService = new PlayerMoveService(_domain);

        // 각 컴포넌트 초기화
        inputHandler.Initialize(_moveService);
        playerBehavior.Initialize(_domain);
    }

    void Update()
    {
        // 매 프레임 클라이언트 예측 이동을 실행
        _moveService.Tick(Time.deltaTime);
        // 테스트 코드
        if (Keyboard.current != null)
        {
            //플레이어 위치를 threshold 이상 이동시켜서 reconcile이 일어나는 것을 체크 (space바로 체크)
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                TestReconcile();
            }

            // 네트워크 지연 상황을 임의로 구현(100ms 이전의 위치가 서버에서 전송한 플레이어의 위치일 때)
            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                Debug.Log("100ms 지연 후 보정 시뮬레이션 시작...");
                StartCoroutine(FakeNetworkDelay());
            }
        }
    }

    private void TestReconcile()
    {
        Debug.Log("--- 서버 보정(Reconcile) 테스트 ---");
        Vector2 currentPos = _domain.Position;
        
        // 현재 위치에서 뒤로 2m 지점을 서버 위치로 가제트
        Vector2 fakeServerPos = currentPos - (_domain.Direction * 2.0f); 
        _moveService.Reconcile(fakeServerPos);
        
        Debug.Log($"보정 전: {currentPos} -> 보정 후: {_domain.Position}");
    }

    private System.Collections.IEnumerator FakeNetworkDelay()
    {
        Vector2 serverPoint = _domain.Position;
        yield return new WaitForSeconds(0.1f); // 100ms 지연
        _moveService.Reconcile(serverPoint);
        Debug.Log("지연 보정 완료!");
    }

    public void OnServerPositionReceived(Vector2 pos) => _moveService.Reconcile(pos);
}