using UnityEngine;

// 플레이어가 생성한 벽(돌)의 수명과 체력을 관리하는 컴포넌트.
// - Initialize로 HP와 수명을 설정받음
// - 시간이 지나거나 HP가 0이 되면 스스로 파괴
public class CoverWall : MonoBehaviour
{
    [Header("Debug Defaults (Initialize를 안 부르면 사용)")]
    [SerializeField] private int _defaultHP = 3;
    [SerializeField] private float _defaultLifetime = 4f;

    private int _currentHP;
    private float _lifetime;
    private float _elapsedTime;

    private bool _initialized;

    private void Awake()
    {
        // 혹시 Initialize를 호출하지 않고 Instantiate만 했을 때 대비
        if (_initialized == false)
        {
            Initialize(_defaultHP, _defaultLifetime);
        }
    }

    // 외부에서 HP/수명을 세팅해주는 함수
    public void Initialize(int hp, float lifetimeSeconds)
    {
        _currentHP = hp;
        _lifetime = lifetimeSeconds;
        _elapsedTime = 0f;
        _initialized = true;
    }

    private void Update()
    {
        UpdateLifetime(Time.deltaTime);
    }

    private void UpdateLifetime(float deltaTime)
    {
        if (_lifetime <= 0f) return;

        _elapsedTime += deltaTime;
        if (_elapsedTime >= _lifetime)
        {
            DestroySelf();
        }
    }

    // 다른 오브젝트(적, 탄환 등)에서 호출할 수 있는 데미지 함수
    public void TakeDamage(int amount)
    {
        _currentHP -= amount;
        if (_currentHP <= 0)
        {
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        // TODO: 파괴 이펙트, 사운드 등 추가 가능
        Destroy(gameObject);
    }
}