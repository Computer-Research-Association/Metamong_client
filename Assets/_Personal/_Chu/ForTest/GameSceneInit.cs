using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 테스트 씬 2 전용 초기화.
/// 씬 2 진입 시 JoinSquare()를 호출한다. (씬 1에서 로그인이 완료된 상태)
/// </summary>
public class GameSceneInit : MonoBehaviour
{
    async Task Start()
    {
        await NetworkCore.Instance.JoinSquare();
    }
}
