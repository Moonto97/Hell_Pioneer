using UnityEngine;

public class SimpleBullet : MonoBehaviour
{
    [SerializeField] private float _defaultSpeed = 10f;
    [SerializeField] private float _defaultLifetime = 3f;
    [SerializeField] private int _bulletDamage = 1;

    private Vector2 _direction;
    private float _speed;
    private float _lifetime;
    private float _age;

    private bool _initialized;

    private const float DirectionEpsilon = 0.0001f;
    private const string MonsterHitBoxTag = "MonsterHitBox";

    public void Initialize(Vector2 direction, float speed, float lifetime)
    {
        if (direction.sqrMagnitude <= DirectionEpsilon)
        {
            return;
        }

        _direction = direction.normalized;
        _speed = speed > 0f ? speed : _defaultSpeed;
        _lifetime = lifetime > 0f ? lifetime : _defaultLifetime;
        _age = 0f;
        _initialized = true;
    }

    private void Update()
    {
        if (_initialized == false)
        {
            return;
        }

        float deltaTime = Time.deltaTime;
        transform.position += (Vector3)(_direction * _speed * deltaTime);

        _age += deltaTime;
        if (_age >= _lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_initialized == false)
        {
            return;
        }

        // 1. 팀 규칙: MonsterHitBox 태그를 가진 히트박스에만 대미지 처리
        if (other.CompareTag(MonsterHitBoxTag) == false)
        {
            Destroy(gameObject);
            return;
        }

        // 2. IDamageable 찾기: 히트박스에 직접 붙어 있거나, 부모에 붙어 있을 수 있음
        IDamageable damageable = null;

        if (other.TryGetComponent<IDamageable>(out var damageableOnHitBox))
        {
            damageable = damageableOnHitBox;
        }
        else
        {
            damageable = other.GetComponentInParent<IDamageable>();
        }

        if (damageable != null)
        {
            damageable.TakeDamage(_bulletDamage);
        }

        Destroy(gameObject);
    }
}
