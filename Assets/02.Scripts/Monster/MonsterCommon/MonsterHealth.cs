using UnityEngine;

public class MonsterHealth : MonoBehaviour, IDamageable
{
    private MonsterStat _monsterStat;
    private int _currentHealth;
    
    private void Awake()
    {
        _monsterStat = GetComponent<MonsterStat>();
    }
    
    private void Start()
    {
        _currentHealth = _monsterStat.MaxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        if (damage <= 0 || _currentHealth <= 0)
        {
            return;
        }

        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        // TODO : 오브젝트 풀링으로 관리
        Destroy(gameObject);
    }
}
