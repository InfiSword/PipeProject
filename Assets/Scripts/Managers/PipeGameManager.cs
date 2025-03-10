using PipeConnectGame.Common;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DifficultyEnum
{
    Easy, Normal, Hard, End
}

public class PipeGameManager : MonoBehaviour
{
    public DifficultyEnum currentStage;
    public int currentLevel;
    public string currentstageName;
    public Color level_Color;

    public  LevelData   now_Level;
    private LevelData[] easy_Level;
    private LevelData[] normal_Level;
    private LevelData[] hard_Level;

    //private LevelList easy_allLevel;
    private const int maxLevel = 20;
    private Dictionary<string, LevelData> all_LevelDicts;

    public List<StageButton> stageButtonList;
    public List<LevelButton> levelButtons;
    public ColorLibrary difficulty_ColorLibrary;
    public ColorLibrary node_ColorLibrary;

    public void Init()
    {
        currentStage = DifficultyEnum.End;
        currentLevel = 1;

        easy_Level = Resources.LoadAll<LevelData>("ScriptableObj/Levels/Easy");
        normal_Level = Resources.LoadAll<LevelData>("ScriptableObj/Levels/Normal");
        hard_Level = Resources.LoadAll<LevelData>("ScriptableObj/Levels/Hard");
        all_LevelDicts = new Dictionary<string, LevelData>();

        for(int i=0; i<maxLevel; i++)
        {
            all_LevelDicts[easy_Level[i].so_LevelName] = easy_Level[i];
            all_LevelDicts[normal_Level[i].so_LevelName] = normal_Level[i];
            //all_LevelDicts[hard_Level[i].so_LevelName] = hard_Level[i];
        }
      
        difficulty_ColorLibrary = Managers.Resource.Load<ColorLibrary>("ScriptableObj/Difficulty_Colors");
        node_ColorLibrary = Managers.Resource.Load<ColorLibrary>("ScriptableObj/Node_Color");
        levelButtons = new List<LevelButton>();
        stageButtonList = new List<StageButton>();

    }

    // 현재 딕셔너리의 해당 키값에 맞는 LevelData를 가져옴.  
    public LevelData GetLevel()
    {
        string levelName = currentStage.ToString() + '_' + currentLevel.ToString();
        if (all_LevelDicts.ContainsKey(levelName))
        {
            return all_LevelDicts[levelName];
        }
        return null;
    }

    // 해당 레벨이 잠겨있는지 확인
    public bool IsLevelUnlocked(int _level)
    {
        string levelName = "Level" + currentStage.ToString() + _level.ToString();
        if (_level == 1)        // 초기 레벨 1은 무조건 열려있어야 함
        {
            PlayerPrefs.SetInt(levelName, 1);
            return true;
        }

        if (PlayerPrefs.HasKey(levelName))
        {
            return PlayerPrefs.GetInt(levelName) == 1;
        }

        PlayerPrefs.SetInt(levelName, 0);
        return false;
    }

    public void UnlockLevel()
    {
        currentLevel++;
        if ((currentLevel < maxLevel + 1))
        {
            string levelName = "Level" + currentStage.ToString() + currentLevel.ToString();
            PlayerPrefs.SetInt(levelName, 1);
        }
        else
        {
            string levelName = "Level" + currentStage.ToString() + currentLevel.ToString();
            PlayerPrefs.SetInt(levelName, 0);
        }
            
    }

    public void ResetAllLevels()
    {
           for (int stage = 0; stage < (int)DifficultyEnum.End; stage++)
        {
            for (int level = 1; level <= maxLevel; level++)
            {
                string levelName = "Level" + (DifficultyEnum)stage + level.ToString();
                PlayerPrefs.SetInt(levelName, 0); // 모든 레벨을 잠금 상태로 
            }
        }

        currentStage = (DifficultyEnum)1;
        currentLevel = 1;

        PlayerPrefs.Save();
    }

}

