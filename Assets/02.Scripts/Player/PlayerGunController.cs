using UnityEngine;

public class PlayerGunController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private WeaponBase _currentWeapon;
    [SerializeField] private Transform _gunPivot;
    [SerializeField] private Camera _camera;

    [Header("Aim Settings")]
    [SerializeField] private bool _invertAim = false;

    [Header("Fire Settings")]
    [SerializeField] private float _defaultFireRate = 3f;

    private float _fireCooldown;
    private Vector2 _currentAimDirection;
    private const float DirectionEpsilon = 0.0001f;

    private void Awake()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        if (_gunPivot == null)
        {
            _gunPivot = transform;
        }

        if (_playerStats == null)
        {
            _playerStats = GetComponent<PlayerStats>();
        }

        _currentAimDirection = Vector2.right;
    }

    private void Update()
    {
        UpdateAim();
        UpdateShooting(Time.deltaTime);
    }

    private void UpdateAim()
    {
        if (_camera == null || _gunPivot == null)
        {
            return;
        }

        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = _camera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector3 dir3 = mouseWorldPos - _gunPivot.position;

        if (dir3.sqrMagnitude <= DirectionEpsilon)
        {
            return;
        }

        Vector2 aimDir = new Vector2(dir3.x, dir3.y).normalized;

        if (_invertAim)
        {
            aimDir = -aimDir;
        }

        _currentAimDirection = aimDir;

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        _gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void UpdateShooting(float deltaTime)
    {
        if (_fireCooldown > 0f)
        {
            _fireCooldown -= deltaTime;
        }

        if (Input.GetMouseButton(0) && _fireCooldown <= 0f)
        {
            TryFireCurrentWeapon();
        }
    }

    private void TryFireCurrentWeapon()
    {
        if (_currentWeapon == null)
        {
            return;
        }

        if (_currentAimDirection.sqrMagnitude <= DirectionEpsilon)
        {
            return;
        }

        float fireRate = GetCurrentFireRate();
        if (fireRate <= 0f)
        {
            return;
        }

        _currentWeapon.Fire(_currentAimDirection, fireRate);

        _fireCooldown = 1f / fireRate;
    }

    private float GetCurrentFireRate()
    {
        if (_playerStats != null)
        {
            return _playerStats.FireRate;
        }

        return _defaultFireRate;
    }

    public void SetCurrentWeapon(WeaponBase newWeapon)
    {
        _currentWeapon = newWeapon;
    }
}
