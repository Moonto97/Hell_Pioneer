using UnityEngine;

namespace Hell_Pioneer.Item
{
    /// <summary>
    /// 바닥에 놓인 "피킹 가능한 아이템"의 공통 베이스 클래스.
    /// - 트리거 충돌 감지
    /// - 플레이어 태그 체크
    /// - 픽업 후 오브젝트 제거/비활성화
    /// 구체적인 효과는 상속받는 클래스에서 HandlePickup을 통해 구현한다.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public abstract class ItemPickup : MonoBehaviour
    {
        [Header("Common Pickup Settings")]
        [SerializeField] private bool _destroyOnPickup = true;

        /// <summary>
        /// 2D 트리거 충돌 시 호출.
        /// 플레이어와 충돌하면 픽업 로직을 수행한다.
        /// </summary>
        /// <param name="other">충돌한 콜라이더</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (CanPickUp(other) == false)
            {
                return;
            }

            HandlePickup(other);

            // 이후 연출(VFX/SFX 등)을 이 안에서 호출하거나, HandlePickup 내부에서 호출하도록 할 수 있음.
            // 현재는 아이템 제거만 담당.
            if (_destroyOnPickup)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 이 오브젝트를 픽업할 수 있는 대상인지 검사한다.
        /// 기본 구현은 "Player" 태그를 가진 오브젝트만 허용한다.
        /// 필요 시 하위 클래스에서 오버라이드 가능.
        /// </summary>
        /// <param name="other">충돌한 콜라이더</param>
        /// <returns>픽업 가능 여부</returns>
        protected virtual bool CanPickUp(Collider2D other)
        {
            return other.CompareTag("Player");
        }

        /// <summary>
        /// 실제 아이템 효과를 적용하는 추상 메서드.
        /// - 최대 체력 증가, 스킬 부여, 자원 회복 등
        /// 하위 클래스에서 구체적인 로직을 구현한다.
        /// </summary>
        /// <param name="other">아이템을 픽업한 대상의 콜라이더</param>
        protected abstract void HandlePickup(Collider2D other);
    }
}
