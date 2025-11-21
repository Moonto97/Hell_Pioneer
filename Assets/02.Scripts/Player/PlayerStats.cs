using UnityEngine;

// 플레이어의 스탯과 체력을 관리하는 컴포넌트.
// - 초기/현재 HP 이동 속도 공속(연사력)
// - 피격 처리, 1초 무적 + 깜빡이기
// - IPlayerHealth, IPlayerFireRate 구현
public class PlayerStats : MonoBehaviour, IPlayerHealth, IPlayerFireRate
{
    [Header("Initial Stats")]
    [SerializeField] private int _initialMaxHp = 10;        //시작 최대 체력
    [SerializeField] private float _initialMoveSpeed = 5f;  //시작 이동 속도
    [SerializeField] private float _initialFireRate = 3f;   //시작 공속. 예: 초당 3발

    private const float MinFireRate = 0.1f;           // 최소 공속 제한 (아예 0 이하로 가는 걸 방지)

    // --- 런타임 상태 ---
    public int MaxHp { get; private set; }      // 최대 체력
    public int CurrentHp { get; private set; }  // 현재 체력

    public float MoveSpeed { get; private set; }
    public float FireRate { get; private set; }

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

    #region IPlayerHealth 구현 + 체력 관련 로직

    public void Heal(float percent)
    {
        // 죽은 상태이거나 회복 비율이 0 이하이면 무시
        if (percent <= 0f)
        {
            return;
        }

        // 최대 체력의 amount 비율만큼 회복량 계산
        int healAmount = Mathf.RoundToInt(MaxHp * percent);

        // 현재 체력에 더하고 MaxHp를 넘지 않도록 클램프
        CurrentHp = Mathf.Clamp(CurrentHp + healAmount, 0, MaxHp);

        // TODO: HP UI 업데이트 이벤트 등

    }

    // 최대 체력을 amount 비율만큼 증가시키는 메서드
    public void IncreaseMaxHealth(float percent)
    {
        if (percent <= 0f)
        {
            return;
        }

        // 현재 MaxHp의 amount 비율만큼 증가량 계산
        int increaseAmount = Mathf.RoundToInt(MaxHp * percent);

        MaxHp += increaseAmount;

        // 기획에 따라: 최대 체력이 늘 때 현재 체력도 함께 올릴지 결정
        CurrentHp += increaseAmount;
        CurrentHp = Mathf.Clamp(CurrentHp, 0, MaxHp);

        // TODO: HP UI 업데이트 이벤트 등
        
    }

    // 외부(적, 탄환 등)에서 피해를 줄 때 호출하는 메서드.
    public void TakeDamage(int amount)
    {
        if (amount <= 0)
            return;

        var hit = GetComponent<PlayerHitFeedback>();
        if (hit != null && hit.IsInvincible)
            return;

        CurrentHp -= amount;

        if (CurrentHp <= 0)
        {
            CurrentHp = 0;

            if (hit != null)
                hit.PlayDeathFeedback();
        }
        else
        {
            if (hit != null)
                hit.StartHitFeedback();
        }

        // TODO: HP UI 업데이트 이벤트 등
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
        if (FireRate < MinFireRate)
        {
            FireRate = MinFireRate;
        }

        // TODO: 무기/발사 시스템에 FireRate 변경 알리기 (이벤트 등)
    }


    #endregion

    #region MoveSpeed 관련 유틸리티

    // 이동 속도를 절대값으로 설정합니다.(버프/디버프 등은 별도 로직에서 관리 가능)
    public void SetMoveSpeed(float newSpeed)
    {
        MoveSpeed = Mathf.Max(0f, newSpeed);
    }

    /// 이동 속도를 amount만큼 가감
    public void AddMoveSpeed(float amount)
    {
        SetMoveSpeed(MoveSpeed + amount);
    }

    #endregion
}
