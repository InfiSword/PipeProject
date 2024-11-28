using UnityEngine;

public class GridGenerator
{
    public int gridWidth;
    public int gridHeight;
    public PathNode[,] grid;

    public GridGenerator(int width, int height)
    {
        gridWidth = width;
        gridHeight = height;
        grid = new PathNode[gridWidth, gridHeight];
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y] = new PathNode(new Vector2Int(x, y));
            }
        }
    }

    public PathNode GetNodeAtPosition(Vector2Int position)
    {
        if (position.x >= 0 && position.x < gridWidth && position.y >= 0 && position.y < gridHeight)
        {
            return grid[position.x, position.y];
        }
        return null;
    }
}
