using UnityEngine;

// 피격 시 무적, 깜빡임, 죽음 연출을 담당하는 컴포넌트
// PlayerStats의 TakeDamage가 호출될 때 이 컴포넌트를 통해 시각 효과 수행
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

    private void Awake()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        SetSpritesVisible(true);
    }

    private void Update()
    {
        UpdateInvincibility(Time.deltaTime);
    }

    public bool IsInvincible => _isInvincible;

    public void StartHitFeedback()
    {
        if (_isDead)
        {
            return;
        }

        _isInvincible = true;
        _invincibleTimer = _hitInvincibleDuration;
        _blinkTimer = 0f;
    }

    public void PlayDeathFeedback()
    {
        _isDead = true;
        SetSpritesVisible(true); // 죽을 때는 깜빡임 종료
        // TODO : 사망 애니메이션, 사운드, 게임오버 처리 등
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
            SetSpritesVisible(true);
        }
    }

    private void SetSpritesVisible(bool visible)
    {
        foreach (var sr in _spriteRenderers)
        {
            if (sr != null) sr.enabled = visible;
        }
    }

    private void ToggleSpritesVisible()
    {
        foreach (var sr in _spriteRenderers)
        {
            sr.enabled = !sr.enabled;
        }
    }
}
