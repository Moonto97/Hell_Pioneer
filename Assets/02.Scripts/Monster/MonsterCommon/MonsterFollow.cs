using UnityEngine;

public class MonsterFollow : MonoBehaviour
{
    private MonsterStat _monsterStat;

    private void Start()
    {
        _monsterStat = GetComponent<MonsterStat>();
    }
    
    void Update()
    {
        Node currentNode = GridManager.Instance.NodeFromWorld(transform.position);
        if (currentNode == null) return;

        Node[,] grid = GridManager.Instance.Grid;
        int sizeX = GridManager.Instance.SizeX;
        int sizeY = GridManager.Instance.SizeY;

        Node best = currentNode;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int nx = currentNode.X + x;
                int ny = currentNode.Y + y;

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

        if (best != null && best.Distance < currentNode.Distance)
        {
            Vector3 dir = (best.WorldPos - transform.position).normalized;
            transform.position += dir * _monsterStat.Speed * Time.deltaTime;
        }
    }
}