using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public float Speed = 2f;

    void Update()
    {
        Node my = GridManager.Instance.NodeFromWorld(transform.position);
        if (my == null) return;

        Node[,] grid = GridManager.Instance.Grid;
        int sizeX = GridManager.Instance.SizeX;
        int sizeY = GridManager.Instance.SizeY;

        Node best = my;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int nx = my.X + x;
                int ny = my.Y + y;

                if (nx < 0 || ny < 0 || nx >= sizeX || ny >= sizeY)
                    continue;

                Node next = grid[nx, ny];
                if (!next.Walkable) continue;

                if (next.Distance < best.Distance)
                {
                    best = next;
                }
            }
        }

        if (best != null && best.Distance < my.Distance)
        {
            Vector3 dir = (best.WorldPos - transform.position).normalized;
            transform.position += dir * Speed * Time.deltaTime;
        }
    }
}