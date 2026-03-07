using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        GameObject[] uiPrefabs = Resources.LoadAll<GameObject>("Prefabs/UI/State");

        _uIDocuments = uiPrefabs
            .Select(prefab =>
            {
                GameObject go = Instantiate(prefab, transform);
                go.name = prefab.name;
                return go.GetComponent<UIDocument>();
            })
            .Where(doc => doc != null)
            .ToArray();

        foreach (var doc in _uIDocuments)
        {
            string name = doc.gameObject.name;

            if (!System.Enum.TryParse(name, out State state))
            {
                Debug.LogWarning($"State enum not found for {name}");
                continue;
            }

            var stateComponent = doc.GetComponent<IState<UIDocument>>();

            if (stateComponent == null)
            {
                Debug.LogWarning($"{name} missing IState component");
                continue;
            }

            _stateDict.Add(state, (doc, stateComponent));
            if (doc.gameObject.activeSelf) SetState(state);
        }
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
