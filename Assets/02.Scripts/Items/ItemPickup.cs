using UnityEngine;

// 바닥에 놓인 "픽업 가능한 아이템"의 공통 베이스 클래스
// - PlayerStats 컴포넌트 체크
// - 픽업 후 오브젝트 제거/비활성화
// 구체적인 효과는 HandlePickup에서 구현
[RequireComponent(typeof(Collider2D))]
public abstract class ItemPickup : MonoBehaviour
{
    [Header("Common Pickup Settings")]
    [SerializeField] private bool _destroyOnPickup = true;

    private void Reset()
    {
        // 자동으로 IsTrigger를 켜 주면 편함 (실수 방지)
        Collider2D collider2D = GetComponent<Collider2D>();
        if (collider2D != null)
        {
            collider2D.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 픽업 가능 대상(태그가 플레이어)인지 검사
        if (CanPickUp(other) == false)
        {
            return;
        }

        HandlePickup(other);

        if (_destroyOnPickup)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // 이 오브젝트를 픽업할 수 있는 대상인지 검사
    protected virtual bool CanPickUp(Collider2D other)
    {
        return other.TryGetComponent<PlayerStats>(out _);
    }

    // 실제 아이템 효과를 적용하는 추상 메서드, 하위 클래스에서 구체 효과 구현
    protected abstract void HandlePickup(Collider2D other);
}
