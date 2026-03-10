using UnityEngine;
using UnityEngine.UIElements;

public class LoginUI : MonoBehaviour, IState<UIDocument>
{
    UIDocument _uIDocument;
    VisualElement _root;
    Button _enterButton;
    public void Enter(UIDocument owner)
    {
        _uIDocument = owner;
        _uIDocument.gameObject.SetActive(true);
    }
    void OnEnable()
    {
        Init();
    }
    void Init()
    {
        if (_uIDocument == null) _uIDocument = transform.gameObject.GetComponent<UIDocument>();
        _root = _uIDocument.rootVisualElement;
        _enterButton = _root.Query<Button>("EnterButton");
        _enterButton.clicked += OnButtonClicked;

    }
    void OnButtonClicked()
    {
        Managers.UI.SetState(UIManager.State.AvatarSettingUI);
    }
    public void Exit()
    {
        if (!_uIDocument) return;
        _uIDocument.gameObject.SetActive(false);
    }

    public void Update()
    {
    }
}
