using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public abstract class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;

    void Awake() { Init(); } // Scene 로드 즉시 초기화 (Start() 에서 다른 스크립트들이 씬 정보 사용)

    protected virtual void Init()
    {
        // UI 처리를 위해 씬에 EventSystem이 없으면 생성
        Object obj = GameObject.FindFirstObjectByType(typeof(EventSystem));
        if (obj == null) Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
    }

    public abstract void Clear();
}
