using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DifficultyMenu : UI_Popup
{
    enum DifficultyMenuButtons
    {
        MainMenuButton,
    }

    enum DifficultyMenuObj
    {
        Difficutly_Content
    }
    private MainMenu mainMenu;
    private GameObject difficulty_Content;

    private void OnEnable()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    public override void Init()
    {
        isCam = true;
        Managers.Ads.ShowBanner();

        base.Init();
        Bind<Button>(typeof(DifficultyMenuButtons));
        Bind<GameObject>(typeof(DifficultyMenuObj));
        GetButton((int)DifficultyMenuButtons.MainMenuButton).BindEvent((PointerEventData data) =>
        {
            mainMenu = FindObjectOfType<MainMenu>();
            mainMenu.gameObject.SetActive(true);
            ClosePopupUI();
        }, Define.UIEvent.Click);

        difficulty_Content = GetObject((int)DifficultyMenuObj.Difficutly_Content);

        if (Managers.Pipe.stageButtonList.Count > 0)
            return;

        for (int i = 0; i < (int)DifficultyEnum.End; i++)
        {
            StageButton stageBtn = Managers.Resource.Instantiate("StageButton").GetComponent<StageButton>();
            stageBtn.difficultyMenu = this;
            stageBtn.stageNumber = i + 1;
            stageBtn.difficulty = (DifficultyEnum)i;

            if (stageBtn.difficulty == DifficultyEnum.Hard)
            {
                stageBtn.gameObject.GetComponent<Button>().enabled = false;
                stageBtn.stageColor = new Color(170f / 255f, 170f / 255f, 170f / 255f, 1f);
            }
            else
                stageBtn.stageColor = Managers.Pipe.difficulty_ColorLibrary.presets[i].color;

            stageBtn.stageName = $"Stage\r\n{(DifficultyEnum)i}";
            stageBtn.Init();

            Managers.Pipe.stageButtonList.Add(stageBtn);
            Managers.Pipe.stageButtonList[i].transform.SetParent(difficulty_Content.transform, false);
            Managers.Pipe.stageButtonList[i].transform.localScale = Vector3.one;
        }

    }

    public override void ClosePopupUI()
    {
        base.ClosePopupUI();
    }
}
