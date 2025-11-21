using UnityEngine;

public class ExplosionAttack : MonoBehaviour, IMonsterAttack
{
    private MonsterStat _monsterStat;
    private MonsterHealth _health;
    private const string PlayerTag = "Player";
    private void Awake()
    {
        _monsterStat = GetComponentInParent<MonsterStat>();
        _health = GetComponentInParent<MonsterHealth>();
    }

    public void Attack()
    {
        if (_monsterStat == null) return;
       
        int attackDamage = _monsterStat.AttackDamage;
        // TODO : 플레이어에게 데미지 입히기

        SelfExplode();
    }

    private void SelfExplode()
    {
        _health.TakeDamage(_monsterStat.MaxHealth);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PlayerTag) == false) return;
        
        Attack();
    }
}
