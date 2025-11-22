using UnityEngine;

public class MonsterFollow : MonoBehaviour
{
    [Header("플레이어에게 최대근접 거리")]
    [SerializeField] private float MaxFollowDistance = 1f;
    [SerializeField] private Transform player;
    private const string PlayerTag = "Player";
    private MonsterStat _monsterStat;

    private void Start()
    {
        _monsterStat = GetComponent<MonsterStat>();
        player = GameObject.FindGameObjectWithTag(PlayerTag).transform;
    }
    
    void Update()
    {
        if (CheckDistanceAvailable() == false) return;
        Node currentNode = GetCurrentNode();
        if (currentNode == null) return;
        GotoNextNode(currentNode);
    }

    private bool CheckDistanceAvailable()
    {
        if (player == null) return false;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= MaxFollowDistance)
            return false;
        
        return true;
    }

    private Node GetCurrentNode()
    {
        return GridManager.Instance.NodeFromWorld(transform.position);
    }
    
    private void GotoNextNode(Node currentNode)
    {
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