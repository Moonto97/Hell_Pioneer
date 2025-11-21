public interface ICoverFlareOwner
{
    // 커버플레어의 "최대 차지 수(탄창 용량)"를 증가
    // 구현체에서 현재 차지 보정도 함께 처리할 수 있다

    //amount: 증가시킬 최대 차지 수
    void IncreaseCoverFlareMaxCharges(int amount);
}
