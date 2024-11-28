using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum Scene
    {
        Unknown,
        MainMenu,
        Game,
        //Login,
        //Lobby,
        //Game,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum MouseEvent
    {
        Press,
        Click,
    }

    public enum CameraMode
    {
        QuarterView,
    }

    // 각 배경음악
    public enum BGM
    {
        Count
    }

    // 각 효과음
    public enum SFX
    {
        Success,
        Count
    }
}
