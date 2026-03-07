using UnityEngine;
public interface IState<T>
{
    void Enter(T owner);
    void Update();
    void Exit();
}