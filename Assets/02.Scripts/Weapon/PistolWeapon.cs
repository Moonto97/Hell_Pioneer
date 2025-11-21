using UnityEngine;

public class PistolWeapon : WeaponBase
{
    [Header("Pistol Settings")]
    [SerializeField] private float _maxSpreadAngle = 5f;   // 탄퍼짐 최대 각도(도 단위)

    public override void Fire(Vector2 aimDirection, float fireRate)
    {
        if (_bulletPrefab == null || _firePoint == null)
        {
            return;
        }

        if (aimDirection.sqrMagnitude <= DirectionEpsilon)
        {
            return;
        }

        Vector2 normalizedDir = aimDirection.normalized;

        // 탄퍼짐 각도 랜덤
        float spreadAngle = Random.Range(-_maxSpreadAngle, _maxSpreadAngle);
        Quaternion spreadRotation = Quaternion.AngleAxis(spreadAngle, Vector3.forward);
        Vector3 rotatedDir3 = spreadRotation * new Vector3(normalizedDir.x, normalizedDir.y, 0f);
        Vector2 finalDir = new Vector2(rotatedDir3.x, rotatedDir3.y);

        GameObject bulletObj = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);

        if (bulletObj.TryGetComponent(out SimpleBullet bullet))
        {
            bullet.Initialize(finalDir, _bulletSpeed, _bulletLifetime);
        }
        else
        {
            // Bullet 스크립트를 안 붙였을 때 최소한 앞으로 나가기라도 하도록 폴백
            if (bulletObj.TryGetComponent(out Rigidbody2D rb))
            {
                rb.linearVelocity = finalDir * _bulletSpeed;
            }
        }
    }
}
