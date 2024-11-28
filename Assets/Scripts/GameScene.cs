using PipeConnectGame.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Game;
        Camera.main.gameObject.AddComponent<CameraResolution>();
        Managers.UI.ShowSceneUI<MainGame>();

    }

    public override void Clear()
    {
        
    }
}
