using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private float inputThreshold = 0.5f; // 조이스틱 입력 민감도
    private PlayerInput playerInput;
    private InputAction moveAction;
    private PlayerMoveService _moveService;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }

    void Update()
    {
        
    }
    void OnEnable()
    {
        // "Move" 액션이 수행될 때(눌리거나 움직일 때)만 OnMoveContext 함수 실행
        moveAction.performed += OnMoveContext;
        // 키를 뗐을 때 실행
        moveAction.canceled += OnMoveContext;
    }

    public void Initialize(PlayerMoveService _moveService)
    {
        this._moveService = _moveService;
    }
    // 시스템이 호출해주는 콜백 함수

    private void OnMoveContext(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (input != Vector2.zero) Debug.Log($"Input Detected: {input}");
        //if (input == Vector2.zero) return; // 입력 값 없으면 무시

        // 입력을 정수 그리드 방향으로 변환 (joystick threshold 이상 움직였을 경우)
        float x = 0;
        float y = 0;
        if (Mathf.Abs(input.x) > inputThreshold) x = Mathf.Sign(input.x);
        if (Mathf.Abs(input.y) > inputThreshold) y = Mathf.Sign(input.y);

        Vector3 direction = new Vector3(x, y, 0);

        // Application 레이어 호출 (사용자 입력 방향을 전달)
        _moveService.SetDirection(direction);
    }
}
