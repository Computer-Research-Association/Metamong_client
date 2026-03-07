using UnityEngine;

public class RemotePlayerController : MonoBehaviour
{
    private Vector3 _targetPos{get; set;}
    private Vector3 _displayPos{get; set;}
    [SerializeField] private float smoothing = 15f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _displayPos = Vector3.Lerp(_displayPos, _targetPos, smoothing * Time.deltaTime);
        transform.position = _displayPos;
    }

    public void Initialize(Vector2 pos)
    {
        _displayPos = new Vector3(pos.x, pos.y, 0);
    }
    public void SetTargetPosition(Vector2 pos)
    {
        _targetPos = new Vector3(pos.x, pos.y, 0);
    }

}
