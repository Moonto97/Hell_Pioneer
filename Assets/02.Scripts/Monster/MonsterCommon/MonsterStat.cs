using UnityEngine;

public class MonsterStat : MonoBehaviour
{
    private const int DefaultMonsterLevel = 1;
    private const int MaxMonsterLevel = 3;
    private const int DefaultMaxHealth = 2;
    private const int DefaultAttackDamage = 1;
    private const float DefaultSpeed = 2f;

    [SerializeField] private int _monsterLevel = DefaultMonsterLevel;
    [SerializeField] private int _maxHealth = DefaultMaxHealth;
    public int MaxHealth => _maxHealth;
    [SerializeField] private int _attackDamage = DefaultAttackDamage;
    public int AttackDamage => _attackDamage;
    [SerializeField] private float _speed = DefaultSpeed;
    public float Speed => _speed;

    public void SetMonsterLevel(int level)
    {
        if(level > MaxMonsterLevel)
            level = MaxMonsterLevel;
        
        _monsterLevel = level;
    }

    public void SetStat()
    {
        switch (_monsterLevel)
        {
            case 1:
                _maxHealth = 2;
                _attackDamage = 1;
                _speed = DefaultSpeed;
                break;
            case 2:
                _maxHealth = 4;
                _attackDamage = 2;
                _speed = DefaultSpeed;
                break;
            case 3:
                _maxHealth = 8;
                _attackDamage = 3;
                _speed = DefaultSpeed;
                break;
        }
    }
}
