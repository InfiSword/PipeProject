using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private GridGenerator gridGenerator;

    public Pathfinding(GridGenerator grid)
    {
        gridGenerator = grid;
    }

    public List<PathNode> FindPath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> openSet = new List<PathNode>();
        HashSet<PathNode> closedSet = new HashSet<PathNode>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            PathNode currentNode = openSet[0];

            // Find the node with the lowest F cost
            for (int i = 1; i < openSet.Count; i++)
            {
                if (GetFCost(openSet[i], startNode, endNode) < GetFCost(currentNode, startNode, endNode) ||
                    (GetFCost(openSet[i], startNode, endNode) == GetFCost(currentNode, startNode, endNode) &&
                    GetHCost(openSet[i], endNode) < GetHCost(currentNode, endNode)))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            foreach (PathNode neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                    continue;

                int newMovementCostToNeighbor = GetGCost(currentNode, startNode) + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < GetGCost(neighbor, startNode) || !openSet.Contains(neighbor))
                {
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        // No path found
        return null;
    }

    private List<PathNode> RetracePath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    private int GetFCost(PathNode node, PathNode startNode, PathNode endNode)
    {
        return GetGCost(node, startNode) + GetHCost(node, endNode);
    }

    private int GetGCost(PathNode node, PathNode startNode)
    {
        return GetDistance(node, startNode);
    }

    private int GetHCost(PathNode node, PathNode endNode)
    {
        return GetDistance(node, endNode);
    }

    private int GetDistance(PathNode a, PathNode b)
    {
        int dstX = Mathf.Abs(a.position.x - b.position.x);
        int dstY = Mathf.Abs(a.position.y - b.position.y);

        return dstX + dstY;
    }

    private List<PathNode> GetNeighbors(PathNode node)
    {
        List<PathNode> neighbors = new List<PathNode>();

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (var direction in directions)
        {
            PathNode neighbor = gridGenerator.GetNodeAtPosition(node.position + direction);
            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
}
