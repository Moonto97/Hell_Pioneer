public interface IPlayerHealth
{
    /// <summary>
    /// 플레이어의 현재 체력을 amount만큼 회복합니다.
    /// (구현체에서 최대체력 초과 여부를 처리)
    /// </summary>
    /// <param name="percent">회복량</param>
    void Heal(float percent);

    /// <summary>
    /// 플레이어의 최대 체력을 amount만큼 증가시킵니다.
    /// (구현체에서 현재 체력 보정, UI 업데이트 등을 처리)
    /// </summary>
    /// <param name="amount">증가시킬 최대 체력량</param>
    void IncreaseMaxHealth(float amount);
}
