using UnityEngine;
using System.Collections.Generic;

public class PlayerDomain
{
    public Vector2 Position { get; set; }
    public Vector2 Direction { get; set; }
    public float Speed { get; } = 5f;

    // 서버 보정을 위한 입력 기록 (Reconciliation용)
    public struct InputFrame { public Vector2 dir; public float dt; }
    public Queue<InputFrame> InputHistory = new Queue<InputFrame>();

    public PlayerDomain() => Position = Vector2.zero;
}