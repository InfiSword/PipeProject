using UnityEngine;

public class PathNode
{
    public Vector2Int position;
    public bool isStartNode;
    public bool isEndNode;
    public bool isWalkable = true;
    public PathNode parent;

    public PathNode(Vector2Int pos)
    {
        position = pos;
    }
}
