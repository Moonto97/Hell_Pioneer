//using Hell_Pioneer.Player;   // IPlayerHealth 위치에 맞게 네임스페이스 조정
using UnityEngine;

namespace Hell_Pioneer.Item
{
    /// <summary>
    /// 플레이어의 최대 체력을 증가시키는 아이템.
    /// - 플레이어가 닿으면 IPlayerHealth를 찾아 최대 체력을 증가시킨다.
    /// - 이후 연출(이펙트, 사운드 등)은 TODO 영역으로 남겨둔다.
    /// </summary>
    public class MaxHealthUpItem : ItemPickup
    {
        [Header("Max Health Up Settings")]
        [SerializeField] private int _maxHealthIncreaseAmount = 1;      // 최대 체력 증가량

        /// <summary>
        /// 플레이어와 충돌 시 호출되는 실제 효과 로직.
        /// </summary>
        /// <param name="other">아이템을 픽업한 대상의 콜라이더</param>
        protected override void HandlePickup(Collider2D other)
        {
            // 아직 플레이어 스탯 관련 구현되지 않았으므로 우선 주석 처리
            // *추후 위에 using Hell_Pioneer.Player; 도 활성화 필요
            /*
            if (other.TryGetComponent<IPlayerHealth>(out var playerHealth) == false)
            {
                Debug.LogWarning(
                    "MaxHealthUpItem: Player collided but no IPlayerHealth found on player object.",
                    this
                );
                return;
            }
            */

            // 1. 최대 체력 증가
            //playerHealth.IncreaseMaxHealth(_maxHealthIncreaseAmount);

            // 2. TODO: 연출 (나중에 구현)
            // - 파티클 이펙트 재생
            // - 사운드 재생
            // - UI 알림 (예: 'Max HP +1')
            // 이런 부분은 별도 컴포넌트나 Event 시스템으로 분리해도 좋음.
        }
    }
}
