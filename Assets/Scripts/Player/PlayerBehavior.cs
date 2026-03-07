using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] private float smoothing = 15f;
    private PlayerDomain _domain;
    private Vector3 _displayPos;

    public void Initialize(PlayerDomain domain)
    {
        _domain = domain;
        _displayPos = transform.position;
    }

    void Update()
    {
        if (_domain == null) return;
        Vector3 targetPos = new Vector3(_domain.Position.x, _domain.Position.y, 0);
        _displayPos = Vector3.Lerp(_displayPos, targetPos, smoothing * Time.deltaTime);
        transform.position = _displayPos;
    }
}