using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public Tilemap ground;
    public Tilemap wall;

    public Node[,] Grid{get; private set;}
    public int SizeX { get; private set; }
    public int SizeY { get; private set; }
    public BoundsInt Bounds {get; private set;}

    private void Awake()
    {
        Instance = this;
        BuildGrid();
    }

    private void BuildGrid()
    {
        Bounds = ground.cellBounds;

        SizeX = Bounds.size.x;
        SizeY = Bounds.size.y;

        Grid = new Node[SizeX, SizeY];

        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                Vector3Int cell = new(Bounds.x + x, Bounds.y + y);

                bool walkable = ground.HasTile(cell) && !wall.HasTile(cell);
                Vector3 world = ground.CellToWorld(cell) + new Vector3(0.5f, 0.5f);

                Grid[x, y] = new Node(walkable, world, x, y);
            }
        }
    }

    public Node NodeFromWorld(Vector3 world)
    {
        Vector3Int cell = ground.WorldToCell(world);
        int gx = cell.x - Bounds.x;
        int gy = cell.y - Bounds.y;

        if (gx < 0 || gx >= SizeX || gy < 0 || gy >= SizeY)
            return null;

        return Grid[gx, gy];
    }

    private void OnDrawGizmos()
    {
        if (Grid == null) return;

        // Distance 색상 계산용
        int maxDist = 1;
        foreach (Node grid in Grid)
        {
            if (grid.Distance != int.MaxValue)
                maxDist = Mathf.Max(maxDist, grid.Distance);
        }

        foreach (Node grid in Grid)
        {
            if (!grid.Walkable)
            {
                // 벽 = 진한 회색
                Gizmos.color = new Color(0.2f, 0.2f, 0.2f, 1f);
                Gizmos.DrawCube(grid.WorldPos, Vector3.one * 0.9f);
                continue;
            }

            if (grid.Distance == int.MaxValue)
            {
                // 도달 불가한 칸 = 검정
                Gizmos.color = Color.black;
                Gizmos.DrawCube(grid.WorldPos, Vector3.one * 0.9f);
                continue;
            }

            // Distance → 색 변환 (가까울수록 파랑, 멀수록 빨강)
            float t = (float)grid.Distance / maxDist;
            Gizmos.color = Color.Lerp(Color.blue, Color.red, t);

            Gizmos.DrawCube(grid.WorldPos, Vector3.one * 0.9f);
        }
    }
}