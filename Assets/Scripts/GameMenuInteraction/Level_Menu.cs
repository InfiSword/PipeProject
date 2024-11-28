using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Level_Menu : UI_Popup
{
    enum Level_MenuObj
    {
        Level_InfoButton,
        Title,
        Title_Text,
        Level_Content,
    }

    public List<LevelButton> levelButtonList = new List<LevelButton>();
    private GameObject level_Content;
    private const int maxStage = 20;
    private DifficultyEnum difficulty;
    private Color difficulty_Color;

    private void OnEnable()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    public override void Init()
    {
        isCam = true;
        base.Init();
        Bind<GameObject>(typeof(Level_MenuObj));
        level_Content = GetObject((int)Level_MenuObj.Level_Content);
    }

    public void SetLevelMenuUI(DifficultyEnum _difficulty, Color _difficulty_Color)
    {
        difficulty = _difficulty;
        difficulty_Color = _difficulty_Color;
        Managers.Pipe.level_Color = difficulty_Color;

        GetObject((int)Level_MenuObj.Title).GetComponent<Image>().color = difficulty_Color;
        GetObject((int)Level_MenuObj.Title_Text).GetComponent<TMP_Text>().text = $"{difficulty}\r\nStage\r\n";
        GetObject((int)Level_MenuObj.Level_InfoButton).GetComponent<Button>().BindEvent
            ((PointerEventData data) =>
            {
                ClosePopupUI();
                Managers.UI.ShowPopupUI<DifficultyMenu>();
            }, Define.UIEvent.Click);

        if (levelButtonList.Count > 0)
        {
            for (int i = 0; i < levelButtonList.Count; i++)
            {
                levelButtonList[i].levelColor = difficulty_Color;
                levelButtonList[i].Init();
            }
        }
        else
        {
            for (int i = 0; i < maxStage; i++)
            {
                LevelButton levelBtn = Managers.Resource.Instantiate("LevelButton").GetComponent<LevelButton>();
                levelBtn.levelColor = difficulty_Color;
                levelBtn.level_Menu = this;
                levelBtn.levelText.text = $"{i + 1}";
                levelBtn.gameObject.name = $"{difficulty} Level_{i + 1}";               

                levelBtn.Init();
                levelButtonList.Add(levelBtn);
                levelButtonList[i].transform.SetParent(level_Content.transform, false);
                levelButtonList[i].transform.localScale = Vector3.one;
            }
        }
        Managers.Pipe.levelButtons = levelButtonList;
    }

    public override void ClosePopupUI()
    {
        base.ClosePopupUI();
    }
}
