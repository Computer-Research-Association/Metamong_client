using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AvatarSettingUI : MonoBehaviour, IState<UIDocument>
{
    UIDocument _uIDocument;
    VisualElement _root;
    List<Button> _buttons = new();
    VisualElement _elementContainer;
    VisualElement _textFieldContainer;
    ToggleButtonGroup _avatarGroup;
    Label _instructionLabel;
    TextField _textField;
    int _selectedAvatar = -1;
    string _nickname = "";

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
        _buttons = _root.Q<VisualElement>("ButtonContainer").Query<Button>().ToList();
        foreach (Button button in _buttons)
        {
            button.clicked += () => OnButtonClicked(button.name);
        }
        _elementContainer = _root.Q<VisualElement>("ElementContainer");
        _instructionLabel = _elementContainer.Q<Label>("InstructionLabel");
        _avatarGroup = _elementContainer.Q<ToggleButtonGroup>("AvatarGroup");
        _avatarGroup.RegisterValueChangedCallback(evt =>
        {
            OnCharacterChanged(evt.newValue);
        });
        _textFieldContainer = _elementContainer.Q<VisualElement>("TextFieldContainer");
        _textField = _textFieldContainer.Q<TextField>();
        Managers.UI.Hide(_textFieldContainer);
        Managers.UI.Hide(_avatarGroup);
    }
    void OnButtonClicked(string button)
    {
        Debug.Log($"{button} button clicked");
        switch (button)
        {
            case "CreateButton":
                if (Managers.UI.IsElementHidden(_avatarGroup))
                {
                    Managers.UI.Show(_avatarGroup);
                    return;
                }
                break;
            case "EnterButton":
                if (_selectedAvatar == -1)
                {
                    Debug.Log("선택된 아바타가 없습니다.");
                    return;
                }
                Debug.Log($"{_selectedAvatar}번 아바타가 선택되었습니다.");
                _instructionLabel.text = "CHOOSE YOUR NICKNAME";
                Managers.UI.Hide(_avatarGroup);
                Managers.UI.Show(_textFieldContainer);
                break;
        }
    }

    void OnCharacterChanged(ToggleButtonGroupState value)
    {
        for (int i = 0; i <= 6; i++)
        {
            if (value[i])
            {
                Debug.Log($"Selected Index: {i}");
                _selectedAvatar = i;
                return;
            }
        }
        _selectedAvatar = -1;
        Debug.Log("Nothing Selected");
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
