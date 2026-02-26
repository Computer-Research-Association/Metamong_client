using UnityEngine;

public class PlayerMoveService
{
    private readonly PlayerDomain _domain;
    //reconciliation을 수행할 최소 오차
    private readonly float _reconcileThreshold = 0.05f;
    private readonly int _maxHistoryCount = 100;

    public PlayerMoveService(PlayerDomain domain) => _domain = domain;

    public void SetDirection(Vector2 dir) 
    { 
        _domain.Direction = dir.normalized; 
        NetworkCore.Instance.SendMove(dir.normalized);
    }

    public void Tick(float deltaTime)
    {
        if (_domain.Direction == Vector2.zero) return;

        Vector2 movement = _domain.Direction * _domain.Speed * deltaTime;
        _domain.Position += movement;

        // Reconciliation을 수행할 떄 사용할 사용자 인풋 로그
        _domain.InputHistory.Enqueue(new PlayerDomain.InputFrame { dir = _domain.Direction, dt = deltaTime });
        if (_domain.InputHistory.Count > _maxHistoryCount) _domain.InputHistory.Dequeue();
    }

    // 서버 패킷 수신 시 호출되는 보정 로직
    public void Reconcile(Vector2 serverPos)
    {
        // 서버 결과와 현재 위치의 차이가 Threshold보다 작으면 무시
        if (Vector2.Distance(_domain.Position, serverPos) < _reconcileThreshold) return;

        // History에 저장된 인풋을 적용하여 현재 위치 다시 측정
        _domain.Position = serverPos;
        foreach (var frame in _domain.InputHistory)
        {
            _domain.Position += frame.dir * _domain.Speed * frame.dt;
        }
    }
}