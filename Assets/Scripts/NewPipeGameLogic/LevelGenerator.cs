using System.Collections.Generic;
using UnityEngine;
using PipeConnectGame.Common;
using System.Linq;

public class LevelGenerator
{
    public LevelData GenerateLevel(int width, int height)
    {
        GridGenerator gridGenerator = new GridGenerator(width, height);
        PathNode startNode, endNode;
        SetStartAndEndPoints(gridGenerator, out startNode, out endNode);

        Pathfinding pathfinding = new Pathfinding(gridGenerator);
        List<PathNode> path = pathfinding.FindPath(startNode, endNode);

        if (path == null)
        {
            // If no path found, retry
            return GenerateLevel(width, height);
        }

        LevelData levelData = new LevelData();
        levelData.so_LevelName = "Generated Level";
        levelData.so_Edges = new List<Edge>();

        Edge edge = new Edge();
        edge.sm_Points = new List<Vector2Int>();
        edge.sm_Points.Add(startNode.position);
        edge.sm_Points.AddRange(path.Select(n => n.position));
        edge.sm_RotateEdgePoints = new List<Vector2Int>();
        edge.sm_RotateEdgeisBend = new List<bool>();

        // Optionally, add rotate edges
        if (path.Count > 2)
        {
            for (int i = 1; i < path.Count - 1; i++)
            {
                edge.sm_RotateEdgePoints.Add(path[i].position);
                // Randomly decide if the rotatable pipe is a bend
                edge.sm_RotateEdgeisBend.Add(Random.value > 0.5f);
            }
        }

        levelData.so_Edges.Add(edge);

        return levelData;
    }

    private void SetStartAndEndPoints(GridGenerator gridGenerator, out PathNode startNode, out PathNode endNode)
    {
        int gridWidth = gridGenerator.gridWidth;
        int gridHeight = gridGenerator.gridHeight;

        startNode = gridGenerator.grid[Random.Range(0, gridWidth), Random.Range(0, gridHeight)];
        endNode = gridGenerator.grid[Random.Range(0, gridWidth), Random.Range(0, gridHeight)];

        while (endNode == startNode)
        {
            endNode = gridGenerator.grid[Random.Range(0, gridWidth), Random.Range(0, gridHeight)];
        }

        startNode.isStartNode = true;
        endNode.isEndNode = true;
    }
}
