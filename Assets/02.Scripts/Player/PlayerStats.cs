using System;
using UnityEngine;

// 플레이어의 스탯과 체력을 관리하는 컴포넌트.
// - 초기/현재 HP, 이동 속도, 공속
// - 피격 처리 (이벤트 기반)
// - 외부 표현(피격/사망 연출)은 이벤트 구독자가 담당
public class PlayerStats : MonoBehaviour, IPlayerHealth, IPlayerFireRate
{
    [Header("Initial Stats")]
    [SerializeField] private int _initialMaxHp = 10;
    [SerializeField] private float _initialMoveSpeed = 5f;
    [SerializeField] private float _initialFireRate = 3f; // 초당 몇 발

    private const float MinFireRate = 0.1f;

    // --- 런타임 상태 ---
    public int MaxHp { get; private set; }
    public int CurrentHp { get; private set; }
    public float MoveSpeed { get; private set; }
    public float FireRate { get; private set; }

    #region Events
    // 데미지 적용 전에 호출. 구독자 중 하나라도 false 반환하면 피해 무효화.
    public event Func<int, bool> OnBeforeDamage;
    // 실제 데미지 적용 후 호출 (damage, currentHp, maxHp)
    public event Action<int, int, int> OnDamageTaken;
    public event Action OnDeath;
    // 회복 (healAmount, currentHp)
    public event Action<int, int> OnHealed;
    // 최대 체력 증가 (increaseAmount, newMaxHp)
    public event Action<int, int> OnMaxHealthIncreased;
    // 연사력 변경 (newFireRate)
    public event Action<float> OnFireRateChanged;
    #endregion

    private void Awake()
    {
        InitializeStats();
    }

    private void InitializeStats()
    {
        MaxHp = _initialMaxHp;
        CurrentHp = MaxHp;
        MoveSpeed = _initialMoveSpeed;
        FireRate = _initialFireRate;
    }

    #region IPlayerHealth 구현

    public void Heal(float percent)
    {
        if (CurrentHp <= 0 || percent <= 0f) return;

        int healAmount = Mathf.RoundToInt(MaxHp * percent);
        int before = CurrentHp;
        CurrentHp = Mathf.Clamp(CurrentHp + healAmount, 0, MaxHp);
        int applied = CurrentHp - before;

        if (applied > 0)
        {
            OnHealed?.Invoke(applied, CurrentHp);
        }
    }

    public void IncreaseMaxHealth(float percent)
    {
        if (percent <= 0f) return;

        int increaseAmount = Mathf.RoundToInt(_initialMaxHp * percent);
        MaxHp += increaseAmount;

        // 기획: 최대체력 증가 시 현재 체력도 동일량 증가 (클램프)
        CurrentHp = Mathf.Clamp(CurrentHp + increaseAmount, 0, MaxHp);

        OnMaxHealthIncreased?.Invoke(increaseAmount, MaxHp);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        if (CurrentHp <= 0) return; // 이미 사망

        // 데미지 적용 가능 여부(무적 등) 이벤트로 위임
        if (OnBeforeDamage != null)
        {
            foreach (Func<int, bool> del in OnBeforeDamage.GetInvocationList())
            {
                try
                {
                    if (!del(amount))
                    {
                        return; // 구독자 하나라도 false → 피해 무시
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    // 예외 발생 시 해당 구독자만 무시하고 계속 진행
                }
            }
        }

        CurrentHp -= amount;

        if (CurrentHp <= 0)
        {
            CurrentHp = 0;
            OnDamageTaken?.Invoke(amount, CurrentHp, MaxHp);
            OnDeath?.Invoke();
        }
        else
        {
            OnDamageTaken?.Invoke(amount, CurrentHp, MaxHp);
        }
    }

    #endregion

    #region IPlayerFireRate 구현

    public void IncreaseFireRate(float amount)
    {
        if (amount <= 0f) return;

        FireRate *= amount;
        FireRate = Mathf.Max(FireRate, MinFireRate);

        OnFireRateChanged?.Invoke(FireRate);
    }

    #endregion

    #region MoveSpeed 유틸

    public void SetMoveSpeed(float newSpeed)
    {
        MoveSpeed = Mathf.Max(0f, newSpeed);
    }

    public void AddMoveSpeed(float amount)
    {
        SetMoveSpeed(MoveSpeed + amount);
    }

    #endregion
}
