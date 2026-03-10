using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AvatarCollection
{
    [SerializeField] private List<AvatarData> _avatars = new();

    public void AddAvatar(AvatarData avatar)
    {
        _avatars.Add(avatar);
    }

    public List<AvatarData> GetAvatars()
    {
        return _avatars;
    }

    public bool HasAvatar(int id)
    {
        return _avatars.Exists(a => a.AvatarId == id);
    }
}
