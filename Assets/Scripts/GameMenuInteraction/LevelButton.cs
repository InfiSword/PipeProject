using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LevelButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image levelImage;
    public TMP_Text levelText;
    public Color levelColor;

    [SerializeField] private Color inActiveColor;

    private bool isLevelUnlocked;
    private int currentLevel;

    public Level_Menu level_Menu;

    public void Init()
    {
        LevelOpened();
        button.onClick.AddListener(Clicked);
    }

    private void LevelOpened()
    {
        string gameObjectName = gameObject.name;
        string[] parts = gameObjectName.Split('_');
        levelText.text = parts[parts.Length-1];
        currentLevel = int.Parse(levelText.text);
        isLevelUnlocked = Managers.Pipe.IsLevelUnlocked(currentLevel);
        levelImage.color = isLevelUnlocked ? levelColor  : inActiveColor;
       
    }

    private void Clicked()
    {
        if (!isLevelUnlocked)
            return;
        level_Menu.ClosePopupUI();
        Managers.Pipe.currentLevel = currentLevel;
        Managers.Pipe.now_Level = Managers.Pipe.GetLevel();     
        Managers.Scene.LoadScene(Define.Scene.Game);

    }
}
