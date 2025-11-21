using UnityEngine;

public class MonsterStat : MonoBehaviour
{
    private const int DefaultMaxHealth = 2;
    private const int DefaultAttackDamage = 1;
    private const float DefaultSpeed = 2f;

    [SerializeField] private int _monsterLevel = 1;
    [SerializeField] private int _maxHealth = DefaultMaxHealth;
    public int MaxHealth => _maxHealth;
    [SerializeField] private int _attackDamage = DefaultAttackDamage;
    public int AttackDamage => _attackDamage;
    [SerializeField] private float _speed = DefaultSpeed;
    public float Speed => _speed;
}
