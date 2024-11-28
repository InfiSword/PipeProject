using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PipeConnectGame.Common
{
    [CreateAssetMenu(fileName = "DefaultLevelList", menuName = "PipeGameSO/LevlList", order = 1)]
    public class LevelList : ScriptableObject
    {
        public List<LevelData> so_Levels;
    }
}