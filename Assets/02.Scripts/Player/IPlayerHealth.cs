public interface IPlayerHealth
{
    // 플레이어의 현재 체력을 amount만큼 회복 (구현체에서 최대체력 초과 여부를 처리)
    //percent: 회복량
    void Heal(float percent);

    // 플레이어의 최대 체력을 amount만큼 증가 (구현체에서 현재 체력 보정, UI 업데이트 등을 처리)
    // name: 증가시킬 최대 체력량
    void IncreaseMaxHealth(float percent);
}
