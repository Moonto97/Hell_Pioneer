using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public float Speed = 2f;

    void Update()
    {
        Node my = GridManager.Instance.NodeFromWorld(transform.position);
        if (my == null) return;

        Node[,] grid = GridManager.Instance.Grid;

        int[,] dirs =
        {
            { 1,  0}, {-1,  0}, { 0, 1}, { 0,-1},
            { 1,  1}, {-1, 1}, { 1,-1}, {-1,-1}
        };

        Node best = null;

        for (int i = 0; i < 8; i++)
        {
            int nx = my.X + dirs[i, 0];
            int ny = my.Y + dirs[i, 1];

            if (nx < 0 || ny < 0 || nx >= GridManager.Instance.SizeX || ny >= GridManager.Instance.SizeY)
                continue;

            Node next = grid[nx, ny];
            if (!next.Walkable) continue;

            // ÄÚ³ÊÄÆ ¹æÁö
            if (dirs[i, 0] != 0 && dirs[i, 1] != 0)
            {
                Node sideA = grid[my.X + dirs[i, 0], my.Y];
                Node sideB = grid[my.X, my.Y + dirs[i, 1]];
                if (!sideA.Walkable || !sideB.Walkable)
                    continue;
            }

            if (best == null || next.Distance < best.Distance)
                best = next;
        }

        if (best != null)
        {
            Vector3 direction = (best.WorldPos - transform.position).normalized;
            transform.position += direction * Speed * Time.deltaTime;
        }
    }
}