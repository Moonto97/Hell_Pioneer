using UnityEngine;

/// <summary>
/// 플레이어의 스탯과 체력을 관리하는 컴포넌트.
/// - 초기/현재 HP
/// - 초기/현재 이동 속도
/// - 초기/현재 공속(연사력)
/// - 피격 처리, 1초 무적 + 깜빡이기
/// - IPlayerHealth, IPlayerFireRate 구현
/// </summary>
public class PlayerStats : MonoBehaviour, IPlayerHealth, IPlayerFireRate
{
    [Header("Initial Stats")]
    [SerializeField] private int _initialMaxHp = 10;        //시작 최대 체력
    [SerializeField] private float _initialMoveSpeed = 5f;  //시작 이동 속도
    [SerializeField] private float _initialFireRate = 3f;   //시작 공속. 예: 초당 3발

    [Header("Hit Invincibility")]
    [SerializeField] private float _hitInvincibleDuration = 1f;     // 피격 후 무적 지속 시간
    [SerializeField] private float _hitBlinkInterval = 0.1f;        // 피격 후 깜빡임 간격

    // --- 런타임 상태 ---
    public int MaxHp { get; private set; }
    public int CurrentHp { get; private set; }

    public float MoveSpeed { get; private set; }
    public float FireRate { get; private set; }

    private bool _isInvincible;
    private float _invincibleTimer;
    private float _blinkTimer;

    // 스프라이트 렌더러들 캐싱. 다만 자식 오브젝트까지 모두 가져오므로 주의
    private SpriteRenderer[] _spriteRenderers;

    private bool _isDead;

    private void Awake()
    {
        // 자식 오브젝트까지 포함한 모든 SpriteRenderer 컴포넌트 캐싱이므로 주의
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        InitializeStats();
    }

    private void Update()
    {
        UpdateInvincibility(Time.deltaTime);
    }

    private void InitializeStats()
    {
        MaxHp = _initialMaxHp;
        CurrentHp = MaxHp;

        MoveSpeed = _initialMoveSpeed;
        FireRate = _initialFireRate;

        _isDead = false;
        _isInvincible = false;
        _invincibleTimer = 0f;
        _blinkTimer = 0f;

        SetSpritesVisible(true);
    }

    #region IPlayerHealth 구현 + 체력 관련 로직

    public void Heal(float amount)
    {
        // 죽은 상태이거나 회복 비율이 0 이하이면 무시
        if (_isDead || amount <= 0f)
        {
            return;
        }

        // 최대 체력의 amount 비율만큼 회복량 계산
        int healAmount = Mathf.RoundToInt(MaxHp * amount);

        // 현재 체력에 더하고 MaxHp를 넘지 않도록 클램프
        CurrentHp = Mathf.Clamp(CurrentHp + healAmount, 0, MaxHp);

        // TODO: HP UI 업데이트 이벤트 등
        Debug.Log($"회복템을 먹었다! MaxHP의 {amount * 100f}% 회복 ( +{healAmount} ) \n 참고로 나는 PlayerStats.cs 소속이야~");
    }

    /// <summary>
    /// 최대 체력을 amount 비율만큼 증가시키는 메서드
    /// </summary>
    public void IncreaseMaxHealth(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        // 현재 MaxHp의 amount 비율만큼 증가량 계산
        int increaseAmount = Mathf.RoundToInt(MaxHp * amount);

        MaxHp += increaseAmount;

        // 기획에 따라: 최대 체력이 늘 때 현재 체력도 함께 올릴지 결정
        CurrentHp += increaseAmount;
        CurrentHp = Mathf.Clamp(CurrentHp, 0, MaxHp);

        // TODO: HP UI 업데이트 이벤트 등
        Debug.Log($"최대 체력이 증가했다! MaxHP의 {amount * 100f}% 증가 ( +{increaseAmount} ) \n 참고로 나는 PlayerStats.cs 소속이야~");
    }

    /// <summary>
    /// 외부(적, 탄환 등)에서 피해를 줄 때 호출하는 메서드.
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (_isDead || _isInvincible || amount <= 0)
        {
            return;
        }

        CurrentHp -= amount;

        if (CurrentHp <= 0)
        {
            CurrentHp = 0;
            Die();
        }
        else
        {
            StartInvincibility();
        }

        // TODO: HP UI 업데이트 이벤트 등
    }

    private void Die()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;
        SetSpritesVisible(true); // 죽을 때는 깜빡임 종료, 보이게

        // TODO: 사망 애니메이션 재생, 입력 막기, GameOver 연동 등
    }

    private void StartInvincibility()
    {
        _isInvincible = true;
        _invincibleTimer = _hitInvincibleDuration;
        _blinkTimer = 0f; // 바로 한 번 토글되도록
    }

    private void UpdateInvincibility(float deltaTime)
    {
        if (_isInvincible == false)
        {
            return;
        }

        _invincibleTimer -= deltaTime;
        _blinkTimer -= deltaTime;

        if (_blinkTimer <= 0f)
        {
            ToggleSpritesVisible();
            _blinkTimer = _hitBlinkInterval;
        }

        if (_invincibleTimer <= 0f)
        {
            _isInvincible = false;
            SetSpritesVisible(true); // 무적 종료 시 항상 보이게
        }
    }

    private void SetSpritesVisible(bool visible)
    {
        if (_spriteRenderers == null)
        {
            return;
        }

        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _spriteRenderers[i].enabled = visible;
        }
    }

    private void ToggleSpritesVisible()
    {
        if (_spriteRenderers == null)
        {
            return;
        }

        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _spriteRenderers[i].enabled = !_spriteRenderers[i].enabled;
        }
    }

    #endregion

    #region IPlayerFireRate 구현 + 공속 관련 로직

    public void IncreaseFireRate(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        // 현재 연사력에 amount 배수를 곱한다. (예: 1.2f → 20% 증가)
        FireRate *= amount;

        // 안전장치: 0 이하로 내려가면 안 되도록
        if (FireRate < 0.1f)
        {
            FireRate = 0.1f;
        }

        // TODO: 무기/발사 시스템에 FireRate 변경 알리기 (이벤트 등)
        Debug.Log($"연사력이 증가했다! 배율: {amount}, 현재 연사력: {FireRate} \n 참고로 나는 PlayerStats.cs 소속이야~");
    }


    #endregion

    #region MoveSpeed 관련 유틸리티

    /// <summary>
    /// 이동 속도를 절대값으로 설정합니다.
    /// (버프/디버프 등은 별도 로직에서 관리 가능)
    /// </summary>
    public void SetMoveSpeed(float newSpeed)
    {
        MoveSpeed = Mathf.Max(0f, newSpeed);
    }

    /// <summary>
    /// 이동 속도를 amount만큼 가감합니다.
    /// </summary>
    public void AddMoveSpeed(float amount)
    {
        SetMoveSpeed(MoveSpeed + amount);
    }

    #endregion
}
