using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    private UIDocument[] _uIDocuments;
    private Stack<State> _stateStack = new();
    private Dictionary<State, (UIDocument doc, IState<UIDocument> state)> _stateDict = new();
    private IState<UIDocument> _state;

    public enum State
    {
        LoginUI
    }

    public void Init()
    {
        foreach (State state in Enum.GetValues(typeof(State)))
        {
            GameObject go = Managers.Resource.Instantiate($"UI/State/{state}", transform);

            if (go == null)
            {
                Debug.LogWarning($"UI Prefab not found : {state}");
                continue;
            }

            UIDocument doc = go.GetComponent<UIDocument>();
            if (doc == null)
            {
                Debug.LogWarning($"{state} missing UIDocument");
                continue;
            }

            var stateComponent = go.GetComponent<IState<UIDocument>>();
            if (stateComponent == null)
            {
                Debug.LogWarning($"{state} missing IState component");
                continue;
            }

            _stateDict.Add(state, (doc, stateComponent));

            go.SetActive(false);
        }
        SetState(State.LoginUI);
    }
    public void SetState(State state)
    {
        _state?.Exit();
        _stateStack.Push(state);

        var data = _stateDict[state];
        _state = data.state;

        _state.Enter(data.doc);
    }

    public void GoBack()
    {
        if (_stateStack.Count <= 1) return;
        _stateStack.Pop();
        State prevState = _stateStack.Peek();
        SetState(prevState);
    }

    public void Show(VisualElement element)
    {
        element.RemoveFromClassList("hidden");
    }

    public void Hide(VisualElement element)
    {
        element.AddToClassList("hidden");
    }
}
