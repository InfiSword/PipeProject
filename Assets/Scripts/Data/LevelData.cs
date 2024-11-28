using System.Collections.Generic;
using UnityEngine;

namespace PipeConnectGame.Common
{
    [CreateAssetMenu(fileName = "DefaultLevelData", menuName = "PipeGameSO/Level", order = 0)]
    public class LevelData : ScriptableObject
    {
        public string so_LevelName;
        public List<Edge> so_Edges;        
    }

    [System.Serializable]
    public struct Edge
    {
        public List<Vector2Int> sm_Points;
        public Vector2Int sm_StartPoint
        {
            get
            {
                if(sm_Points != null && sm_Points.Count > 0)
                {
                    return sm_Points[0];
                }
                return new Vector2Int(-1, -1);
            }
        }
        public Vector2Int sm_EndPoint
        {
            get
            {
                if (sm_Points != null && sm_Points.Count > 0)
                {
                    return sm_Points[sm_Points.Count - 1];
                }
                return new Vector2Int(-1, -1);
            }
        }
        public List<Vector2Int> sm_RotateEdgePoints;        // 회전 파이프 위치 리스트
        public List<bool> sm_RotateEdgeisBend;              // 회전 파이프가 회전 파이프인가?
    }
}
