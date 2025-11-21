public interface IPlayerFireRate
{
    // 플레이어의 연사력을 증가시키는 메서드, amount 해석은 구현체에 따라 다를 수 있음
    // (예: 발사 간격 감소, 발사 속도 배율 증가 등)
    /// <param name="amount">연사력 증가량(비율 또는 절대값)</param>
    void IncreaseFireRate(float amount);
}
