using UnityEngine;

public class SimpleBullet : MonoBehaviour
{
    [SerializeField] private float _defaultSpeed = 10f;
    [SerializeField] private float _defaultLifetime = 3f;
    [SerializeField] private float _bulletDamage = 1f;

    private Vector2 _direction;
    private float _speed;
    private float _lifetime;
    private float _age;

    private bool _initialized;

    public void Initialize(Vector2 direction, float speed, float lifetime)
    {
        if (direction.sqrMagnitude <= 0f)
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
}
