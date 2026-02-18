using System;
using Metamong.Core;
using UnityEngine;

public class UserManager
{
    private UserData _currentUser;

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

    public event Action<UserData> OnUserDataChanged;

    public void SetUser(UserData userData)
    {
        CurrentUser = userData;
    }

    public void ClearUser()
    {
        CurrentUser = null;
    }

    public void UpdateUser(UserData updatedUserData)
    {
        if (_currentUser == null)
        {
            Debug.LogWarning("[UserManager] No current user");
            return;
        }

        CurrentUser = updatedUserData;
    }

    public bool IsNewUser()
    {
        return _currentUser != null && _currentUser.Status == UserStatus.NEW;
    }
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

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(_currentUser);
        PlayerPrefs.SetString("user_data", json);
        PlayerPrefs.Save();
        Debug.Log("[UserManager] User data saved locally");
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
