using UnityEngine;

public class MonsterStat: MonoBehaviour
{
    [SerializeField] private int _monsterLevel = 1;
    [SerializeField] private int _maxHealth = 2;
    public int MaxHealth => _maxHealth;
    [SerializeField] private int _attackDamage = 1;
    public int AttackDamage => _attackDamage;
    [SerializeField] private float _speed = 2f;
    public float Speed => _speed;
}
