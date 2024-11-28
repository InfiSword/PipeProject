using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : UI_Scene
{
    enum MenuButtons
    {
        Start,
        End,
        Reset,
    }

    public override void Init()
    {
        isCam = true;         
        base.Init();        
        Bind<Button>(typeof(MenuButtons));
        GetButton((int)MenuButtons.Start).BindEvent((PointerEventData data) =>
        {
            Managers.UI.ShowPopupUI<DifficultyMenu>();
            gameObject.SetActive(false);
        }, Define.UIEvent.Click);

        GetButton((int)MenuButtons.Reset).BindEvent((PointerEventData data) =>
        {
            Managers.Pipe.ResetAllLevels();
        }, Define.UIEvent.Click);

        GetButton((int)MenuButtons.End).BindEvent((PointerEventData data) =>
        {            
            Application.Quit();
        }, Define.UIEvent.Click);
    }
}
