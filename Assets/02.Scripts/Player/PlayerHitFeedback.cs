using UnityEngine;

public class PlayerHitFeedback : MonoBehaviour
{
    [Header("피격 설정")]
    [SerializeField] private float _hitInvincibleDuration = 1f;
    [SerializeField] private float _hitBlinkInterval = 0.1f;

    private bool _isInvincible;
    private bool _isDead;
    private float _invincibleTimer;
    private float _blinkTimer;
    private SpriteRenderer[] _spriteRenderers;

    private PlayerStats _stats;

    public bool IsInvincible => _isInvincible;

    private void Awake()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        _stats = GetComponent<PlayerStats>();

        if (_stats != null)
        {
            // 데미지 적용 전: 무적이면 false 반환하여 피해 취소
            _stats.OnBeforeDamage += HandleBeforeDamage;
            // 실제 피해 적용 후: 깜빡임 시작
            _stats.OnDamageTaken += HandleDamageTaken;
            // 사망: 사망 연출
            _stats.OnDeath += HandleDeath;
        }
    }

    private void OnDestroy()
    {
        if (_stats != null)
        {
            _stats.OnBeforeDamage -= HandleBeforeDamage;
            _stats.OnDamageTaken -= HandleDamageTaken;
            _stats.OnDeath -= HandleDeath;
        }
    }

    private void Update()
    {
        if (_isDead) return;

        UpdateInvincibility(Time.deltaTime);
    }

    private bool HandleBeforeDamage(int dmg)
    {
        // 무적이면 피해 무효화
        return !_isInvincible && !_isDead;
    }

    private void HandleDamageTaken(int dmg, int currentHp, int maxHp)
    {
        if (_isDead) return;
        StartHitFeedback();
    }

    private void HandleDeath()
    {
        PlayDeathFeedback();
    }

    public void StartHitFeedback()
    {
        _isInvincible = true;
        _invincibleTimer = _hitInvincibleDuration;
        _blinkTimer = 0f;
        SetSpritesVisible(true);
    }

    public void PlayDeathFeedback()
    {
        _isDead = true;
        _isInvincible = false;
        SetSpritesVisible(false);
        // 추가 사망 이펙트 / 애니메이션 트리거 가능
    }

    private void UpdateInvincibility(float deltaTime)
    {
        if (!_isInvincible) return;

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
            SetSpritesVisible(true);
        }
    }

    private void ApplyToSprites(System.Action<SpriteRenderer> action)
    {
        if (_spriteRenderers == null) return;
        foreach (var sr in _spriteRenderers)
        {
            if (sr != null)
            {
                action(sr);
            }
        }
    }

    private void SetSpritesVisible(bool visible)
    {
        ApplyToSprites(sr => sr.enabled = visible);
    }

    private void ToggleSpritesVisible()
    {
        ApplyToSprites(sr => sr.enabled = !sr.enabled);
    }
}