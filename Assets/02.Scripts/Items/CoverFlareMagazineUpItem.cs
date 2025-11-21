using UnityEngine;

public class CoverFlareMagazineUpItem : ItemPickup
{
    [Header("Cover Flare Magazine Up Settings")]
    [SerializeField] private int _maxChargesIncreaseAmount = 1;

    protected override void HandlePickup(Collider2D other)
    {
        if (other.TryGetComponent<ICoverFlareOwner>(out var coverFlareOwner) == false)
        {
            Debug.LogWarning($"CoverFlareMagazineUpItem: Player '{other.name}' collided but no ICoverFlareOwner found.", this);
            return;
        }

        coverFlareOwner.IncreaseCoverFlareMaxCharges(_maxChargesIncreaseAmount);

        // TODO: 이펙트, 사운드, UI: "Cover Flare Max Charges +1" 등
    }
}
