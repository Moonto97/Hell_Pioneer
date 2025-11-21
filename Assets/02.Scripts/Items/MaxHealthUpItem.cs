using UnityEngine;

// 플레이어의 최대 체력을 증가시키는 장비형/패시브 아이템
public class MaxHealthUpItem : ItemPickup
{
    [Header("Max Health Up Settings")]
    [SerializeField] private float _maxHealthIncreasePercent = 0.1f;

    protected override void HandlePickup(Collider2D other)
    {
        if (other.TryGetComponent<IPlayerHealth>(out var playerHealth) == false)
        {
            Debug.LogWarning(
                "MaxHealthUpItem: Player collided but no IPlayerHealth found on player object.",
                this
            );
            return;
        }

        playerHealth.IncreaseMaxHealth(_maxHealthIncreasePercent);

        // TODO: 이펙트, 사운드, UI: "Max HP +1" 등
    }
}
