using UnityEngine;

// 플레이어의 연사력을 증가시키는 장비형/패시브 아이템
// 실제 증가 방식(발사 간격 감소, 배율 증가 등)은 IPlayerFireRate 구현체에 맡긴다
public class FireRateUpItem : ItemPickup
{
    [Header("Fire Rate Up Settings")]
    [SerializeField] private float _fireRateMultiplier = 1.2f;      // 연사력 증가량

    protected override void HandlePickup(Collider2D other)
    {
        if (other.TryGetComponent<IPlayerFireRate>(out var playerFireRate) == false)
        {
            Debug.LogWarning(
                "FireRateUpItem: Player collided but no IPlayerFireRate found on player object.",
                this
            );
            return;
        }

        playerFireRate.IncreaseFireRate(_fireRateMultiplier);

        // TODO: 연사력 증가 이펙트, 사운드, UI 안내
    }
}
