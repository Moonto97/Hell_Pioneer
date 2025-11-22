using UnityEngine;


public class GridBlock : MonoBehaviour
{
    private Collider2D _coller;

    private void Awake()
    {
        _coller = GetComponent<Collider2D>();
    }

    private void Start()
    {
        ApplyToGrid();
    }
    
    /// 이 CoverWall이 차지하는 모든 타일을 Walkable = false로 설정
    private void ApplyToGrid()
    {
        GridManager gridManager = GridManager.Instance;
        if (gridManager == null)
        {
            Debug.LogError("GridManager 없음. Grid 적용 실패");
            return;
        }

        Bounds bounds = _coller.bounds;

        // bounds 안의 모든 타일 스캔
        for (float x = bounds.min.x; x <= bounds.max.x; x += 0.1f)
        {
            for (float y = bounds.min.y; y <= bounds.max.y; y += 0.1f)
            {
                Vector3 worldPos = new Vector3(x, y, 0);
                Node node = gridManager.NodeFromWorld(worldPos);

                if (node == null) continue;
                node.ChangeWalkable(false);
            }
        }
    }
}