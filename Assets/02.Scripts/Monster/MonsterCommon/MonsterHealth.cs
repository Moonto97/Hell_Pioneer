using UnityEngine;

public class MonsterHealth : MonoBehaviour
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
