using UnityEngine;
using System.Collections.Generic;

public class BFSManager : MonoBehaviour
{
    public static BFSManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void BuildDistanceField(Vector3 targetWorld)
    {
        Node[,] grid = GridManager.Instance.Grid;
        int sizeX = GridManager.Instance.SizeX;
        int sizeY = GridManager.Instance.SizeY;

        // 모든 노드 초기화
        foreach (Node n in grid)
            n.Distance = int.MaxValue;

        Node target = GridManager.Instance.NodeFromWorld(targetWorld);
        if (target == null) return;

        Queue<Node> nodeQueue = new Queue<Node>();
        target.Distance = 0;
        nodeQueue.Enqueue(target);

        int[,] dirs =
        {
            { 1,  0}, {-1,  0}, { 0, 1}, { 0,-1}, // 4방향
            { 1,  1}, {-1, 1}, { 1,-1}, {-1,-1}  // 대각선 4방향
        };

        while (nodeQueue.Count > 0)
        {
            Node current = nodeQueue.Dequeue();

            for (int i = 0; i < 8; i++)
            {
                int dx = dirs[i, 0];
                int dy = dirs[i, 1];

                int nx = current.X + dx;
                int ny = current.Y + dy;

                if (nx < 0 || ny < 0 || nx >= sizeX || ny >= sizeY)
                    continue;

                Node next = grid[nx, ny];
                if (!next.Walkable) continue;

                //  코너컷 방지: 대각선 이동 시 양 옆이 막혀 있으면 이동 불가
                if (dx != 0 && dy != 0)
                {
                    Node sideA = grid[current.X + dx, current.Y];
                    Node sideB = grid[current.X, current.Y + dy];

                    if (!sideA.Walkable || !sideB.Walkable)
                        continue;
                }

                int newDist = current.Distance + 1;

                if (newDist < next.Distance)
                {
                    next.Distance = newDist;
                    nodeQueue.Enqueue(next);
                }
            }
        }
    }
}