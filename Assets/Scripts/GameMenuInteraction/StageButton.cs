using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class StageButton : MonoBehaviour
{
    public string stageName;
    public Color stageColor;
    public int stageNumber;
    public DifficultyEnum difficulty;
    private Button button;
    [SerializeField] private Image image;

    public DifficultyMenu difficultyMenu { private get; set; }
    public void Init()
    {
        button = GetComponent<Button>();
        GetComponentInChildren<TMP_Text>(true).text = stageName;
        gameObject.name = $"Stage_{stageNumber}Button";
        image.color = stageColor;
        image.SetAllDirty();

        button.onClick.AddListener(ClickedBuitton);
    }

    private void ClickedBuitton()
    {
        Managers.Pipe.currentStage = difficulty;
        Managers.Pipe.currentstageName = stageName;
        difficultyMenu.ClosePopupUI();
        Level_Menu level_Menu = Managers.UI.ShowPopupUI<Level_Menu>();    
        level_Menu.SetLevelMenuUI(difficulty, stageColor);
    }
}
