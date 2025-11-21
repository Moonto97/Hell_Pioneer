using UnityEngine;

/// <summary>
/// 플레이어의 체력을 회복시키는 소모성 아이템.
/// </summary>
public class HealItem : ItemPickup
{
    [Header("Heal Settings")]
    [SerializeField] private float _healPercent = 0.3f;      // 회복 배율

    protected override void HandlePickup(Collider2D other)
    {
        if (other.TryGetComponent<IPlayerHealth>(out var playerHealth) == false)
        {
            Debug.LogWarning(
                "HealItem: Player collided but no IPlayerHealth found on player object.",
                this
            );
            return;
        }

        // 1. 체력 회복
        playerHealth.Heal(_healPercent);

        // 2. TODO: 회복 이펙트, 사운드, UI 메세지 등
    }
}
