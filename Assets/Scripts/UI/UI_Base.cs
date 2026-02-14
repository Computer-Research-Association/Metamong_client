using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;

public abstract class UI_Base : MonoBehaviour
{
    // UI 요소들을 Dictionary 로 관리함
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();
    public abstract void Init();


    /// <summary>
    /// UI 요소를 자동으로 찾아서 바인딩 해주는 함수
    /// </summary>
    /// <typeparam name="T">GameObject 혹은 UI 요소 (Button, Text, etc.)</typeparam>
    /// <param name="type">UI 요소들을 Enum으로 정의 (ex- typeof(Buttons))</param>
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);

        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length]; // Enum에 정의된 UI 요소 개수만큼
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, names[i], recursive: true);
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], recursive: true);

            if (objects[i] == null)
                Debug.Log($"Failed to Bind() : {names[i]}");
        }
    }

    /// <summary>
    /// Bind 된 UI 요소 가져오기
    /// </summary>
    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;

        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[idx] as T;
    }

    /// <summary>
    /// 이벤트 등록하는 함수
    /// </summary>
    protected void BindEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent eventType = Define.UIEvent.Click)
    {
        UI_EventHandler eventHandler = Util.GetOrAddComponent<UI_EventHandler>(go);

        switch (eventType)
        {
            case Define.UIEvent.Click:
                eventHandler.OnClickHandler -= action;
                eventHandler.OnClickHandler += action;
                break;
            case Define.UIEvent.Drag:
                eventHandler.OnDragHandler -= action;
                eventHandler.OnDragHandler += action;
                break;
        }
    }
}