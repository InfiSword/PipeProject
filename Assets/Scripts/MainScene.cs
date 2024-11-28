using UnityEditor;
using UnityEngine;

public class MainScene : BaseScene
{
    public static bool isSelectMenu;
    public static string menu_Name;
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.MainMenu;
        Camera.main.gameObject.AddComponent<CameraResolution>();
        Managers.UI.ShowSceneUI<MainMenu>();
        if (isSelectMenu)
        {
            switch (menu_Name)
            {
                case "Level_Menu":
                    Managers.UI.ShowPopupUI<Level_Menu>().SetLevelMenuUI(Managers.Pipe.currentStage, Managers.Pipe.level_Color);
                    break;
                case "DifficultyMenu":
                    Managers.UI.ShowPopupUI<DifficultyMenu>();  
                    break;
            }
            isSelectMenu = false;
            menu_Name = null;
        }
    }


    public override void Clear()
    {

    }
}
