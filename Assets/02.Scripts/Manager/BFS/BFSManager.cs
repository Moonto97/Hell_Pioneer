using UnityEngine;
using System.Collections.Generic;

public class BFSManager : MonoBehaviour
{
    public static BFSManager Instance { get; private set; }

    private static readonly int[,] dirs =
    {
        { 1,  0}, {-1,  0}, { 0, 1}, { 0,-1},   // 상하좌우
        { 1,  1}, {-1, 1}, { 1,-1}, {-1,-1}    // 대각선 4방향
    };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private IEnumerable<Node> GetNeighbors(Node current)
    {
        Node[,] grid = GridManager.Instance.Grid;
        int sizeX = GridManager.Instance.SizeX;
        int sizeY = GridManager.Instance.SizeY;

        for (int i = 0; i < 8; i++)
        {
            int dx = dirs[i, 0];
            int dy = dirs[i, 1];

            int nx = current.X + dx;
            int ny = current.Y + dy;

            // 범위 체크
            if (nx < 0 || ny < 0 || nx >= sizeX || ny >= sizeY)
                continue;

            Node next = grid[nx, ny];

            // 이동 불가
            if (!next.Walkable)
                continue;

            // 대각선이면 코너 컷 방지
            if (dx != 0 && dy != 0)
            {
                Node sideA = grid[current.X + dx, current.Y];
                Node sideB = grid[current.X, current.Y + dy];

                if (!sideA.Walkable || !sideB.Walkable)
                    continue;
            }

            yield return next;
        }
    }

    public void BuildDistanceField(Vector3 targetWorld)
    {
        Node[,] grid = GridManager.Instance.Grid;
        int sizeX = GridManager.Instance.SizeX;
        int sizeY = GridManager.Instance.SizeY;

        // 거리 초기화
        foreach (Node n in grid)
            n.Distance = int.MaxValue;

        Node target = GridManager.Instance.NodeFromWorld(targetWorld);
        if (target == null) return;

        Queue<Node> q = new Queue<Node>();
        target.Distance = 0;
        q.Enqueue(target);

        while (q.Count > 0)
        {
            Node current = q.Dequeue();

            foreach (Node next in GetNeighbors(current))
            {
                int newDist = current.Distance + 1;

                if (newDist < next.Distance)
                {
                    next.Distance = newDist;
                    q.Enqueue(next);
                }
            }
        }
    }
}