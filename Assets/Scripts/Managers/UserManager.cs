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
}
