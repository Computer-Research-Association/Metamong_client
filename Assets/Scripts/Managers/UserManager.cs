using UnityEngine;
using Metamong.Core;

public class UserManager
{
    private UserData _currentUser;
    private bool _initialized = false;

    public UserData CurrentUser
    {
        get => _currentUser;
        private set
        {
            _currentUser = value;
            OnUserDataChanged?.Invoke(value);
        }
    }

    public bool HasUser => _currentUser != null;

    // 유저 데이터 변경 이벤트
    public event System.Action<UserData> OnUserDataChanged;

    // Constructor
    public UserManager()
    {
        Debug.Log("[UserManager] Created (lazy initialization)");
    }

    /// <summary>
    /// 유저 데이터 설정
    /// </summary>
    public void SetUser(UserData userData)
    {
        CurrentUser = userData;
        Debug.Log($"[UserManager] User set: {userData.Nickname} (ID: {userData.Id})");
    }

    /// <summary>
    /// 유저 데이터 초기화
    /// </summary>
    public void ClearUser()
    {
        CurrentUser = null;
        Debug.Log("[UserManager] User data cleared");
    }

    /// <summary>
    /// 유저 정보 업데이트
    /// </summary>
    public void UpdateUser(UserData updatedData)
    {
        if (_currentUser == null)
        {
            Debug.LogWarning("[UserManager] No current user to update");
            return;
        }

        CurrentUser = updatedData;
        Debug.Log($"[UserManager] User updated: {updatedData.Nickname}");
    }

    /// <summary>
    /// 닉네임 업데이트
    /// </summary>
    public void UpdateNickname(string newNickname)
    {
        if (_currentUser == null) return;

        _currentUser.Nickname = newNickname;
        OnUserDataChanged?.Invoke(_currentUser);
        Debug.Log($"[UserManager] Nickname updated: {newNickname}");
    }

    /// <summary>
    /// RC 업데이트
    /// </summary>
    public void UpdateRC(RC newRC)
    {
        if (_currentUser == null) return;

        _currentUser.Rc = newRC;
        OnUserDataChanged?.Invoke(_currentUser);
        Debug.Log($"[UserManager] RC updated: {newRC}");
    }

    /// <summary>
    /// 상태 업데이트
    /// </summary>
    public void UpdateStatus(UserStatus newStatus)
    {
        if (_currentUser == null) return;

        _currentUser.Status = newStatus;
        OnUserDataChanged?.Invoke(_currentUser);
        Debug.Log($"[UserManager] Status updated: {newStatus}");
    }

    /// <summary>
    /// 신규 유저 확인
    /// </summary>
    public bool IsNewUser()
    {
        return _currentUser != null && _currentUser.Status == UserStatus.NEW;
    }

    /// <summary>
    /// 활성 유저 확인
    /// </summary>
    public bool IsActiveUser()
    {
        return _currentUser != null && _currentUser.Status == UserStatus.ACTIVE;
    }

    /// <summary>
    /// 유저 정보를 JSON으로 저장 (PlayerPrefs)
    /// </summary>
    public void SaveUserDataLocally()
    {
        if (_currentUser == null)
        {
            Debug.LogWarning("[UserManager] No user data to save");
            return;
        }

        try
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(_currentUser);
            PlayerPrefs.SetString("user_data", json);
            PlayerPrefs.Save();
            Debug.Log("[UserManager] User data saved locally");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[UserManager] Failed to save user data: {ex.Message}");
        }
    }

    /// <summary>
    /// 로컬에 저장된 유저 정보 로드 (PlayerPrefs)
    /// </summary>
    public UserData LoadUserDataLocally()
    {
        if (!PlayerPrefs.HasKey("user_data"))
        {
            Debug.Log("[UserManager] No saved user data found");
            return null;
        }

        try
        {
            string json = PlayerPrefs.GetString("user_data");
            UserData userData = Newtonsoft.Json.JsonConvert.DeserializeObject<UserData>(json);

            if (userData != null)
            {
                CurrentUser = userData;
                Debug.Log($"[UserManager] User data loaded: {userData.Nickname}");
            }

            return userData;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[UserManager] Failed to load user data: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 로컬 유저 데이터 삭제
    /// </summary>
    public void ClearLocalUserData()
    {
        PlayerPrefs.DeleteKey("user_data");
        PlayerPrefs.Save();
        Debug.Log("[UserManager] Local user data cleared");
    }
}