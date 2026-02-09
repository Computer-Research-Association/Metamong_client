using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float inputThreshold = 0.5f; // 조이스틱 입력 민감도

    private PlayerInput playerInput;
    private InputAction moveAction;

    private bool isMoving = false;
    private Vector3 targetPos;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving) return; // 이동 중 무시

        Vector2 input = moveAction.ReadValue<Vector2>(); // NewInputSystem 에서 Vector2 값 읽기

        // 입력 확인용 로그 (동작 안 하면 콘솔창 확인)
        if (input != Vector2.zero) Debug.Log($"Input Detected: {input}");

        if (input == Vector2.zero) return; // 입력 값 없으면 무시


        // 입력을 정수 그리드 방향으로 변환 (joystick threshold 이상 움직였을 경우)
        float x = 0;
        float y = 0;
        if (Mathf.Abs(input.x) > inputThreshold) x = Mathf.Sign(input.x);
        if (Mathf.Abs(input.y) > inputThreshold) y = Mathf.Sign(input.y);

        Vector3 direction = new Vector3(x, y, 0);
        if (direction != Vector3.zero)
        {
            StartCoroutine(MoveRoutine(direction));
        }
    }

    private IEnumerator MoveRoutine(Vector3 direction)
    {
        targetPos = transform.position + direction;

        // 벽 체크 (Physics2D)
        if (Physics2D.OverlapCircle(targetPos, 0.2f, obstacleLayer))
        {
            yield break; // 벽이 있으면 이동 안 함
        }

        isMoving = true;

        // 목표 지점까지 부드럽게 이동
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos; // 위치 보정
        isMoving = false;
    }
}
