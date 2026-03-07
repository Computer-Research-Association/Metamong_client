using System.ComponentModel;
using UnityEngine;

public class Define
{
    public enum AuthProvider
    {
        [Description("google")] Google,
        [Description("kakao")] Kakao,
        [Description("naver")] Naver
    }

    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game,
    }

    public enum UIEvent
    {
        Click, Drag,
    }
}
