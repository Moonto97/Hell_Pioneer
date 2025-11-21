using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Transform _firePoint;
    [SerializeField] protected GameObject _bulletPrefab;

    [Header("Bullet Settings")]
    [SerializeField] protected float _bulletSpeed = 10f;
    [SerializeField] protected float _bulletLifetime = 3f;

    // 방향 벡터가 거의 0인지 판단할 때 쓰는 기준값
    protected const float DirectionEpsilon = 0.0001f;

    // aimDirection: 총이 쏠 방향 (정규화된 벡터가 들어오는 것을 기대)
    // fireRate    : 초당 발사 수 (필요하면 하위 클래스에서 활용)
    public abstract void Fire(Vector2 aimDirection, float fireRate);
}
