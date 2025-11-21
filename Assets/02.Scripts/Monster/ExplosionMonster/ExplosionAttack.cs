using Unity.VisualScripting;
using UnityEngine;

public class ExplosionAttack : MonoBehaviour
{
    private MonsterStat _monsterStat;
    private MonsterHealth _health;

    private void Start()
    {
        _monsterStat = GetComponentInParent<MonsterStat>();
        _health = GetComponentInParent<MonsterHealth>();
    }

    private void Attack()
    {
        if (_monsterStat == null) return;
       
        int attackDamage = _monsterStat.AttackDamage;
        // TODO : 플레이어에게 데미지 입히기
        
        // 자폭
        _health.TakeDamage(_monsterStat.MaxHealth);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        Attack();
    }
}
