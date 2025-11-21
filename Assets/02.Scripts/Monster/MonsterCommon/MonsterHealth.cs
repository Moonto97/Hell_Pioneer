using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    private MonsterStat _monsterStat;
    private int _currentHealth;
    public event System.Action<MonsterHealth> OnMonsterDied;
    
    private void Awake()
    {
        _monsterStat = GetComponent<MonsterStat>();
    }
    
    private void Start()
    {
        ResetHealth();
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
        OnMonsterDied?.Invoke(this);
    }
    
    public void ResetHealth()
    {
        _currentHealth = _monsterStat.MaxHealth;
    }
}
