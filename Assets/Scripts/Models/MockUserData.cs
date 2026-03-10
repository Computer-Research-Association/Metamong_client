using UnityEngine;

[CreateAssetMenu(fileName = "MockUserData", menuName = "UserData/MockUserData")]
public class MockUserData: ScriptableObject
{
    public AvatarCollection Avatars;// { get; private set; } = new();
}
