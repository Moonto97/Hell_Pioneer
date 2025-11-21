using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MonsterLevelData
{
    public int MaxHealth;
    public int AttackDamage;
    public float Speed;
}
public class MonsterStat : MonoBehaviour
{
    private int _level = 1;
    private static Dictionary<int, MonsterLevelData> _levelData =
        new Dictionary<int, MonsterLevelData>()
        {
            {1, new MonsterLevelData { MaxHealth = 2, AttackDamage = 1, Speed = 2f }},
            {2, new MonsterLevelData { MaxHealth = 4, AttackDamage = 2, Speed = 2f }},
            {3, new MonsterLevelData { MaxHealth = 8, AttackDamage = 3, Speed = 2f }},
        };

    private int _maxHealth;
    public int MaxHealth => _maxHealth;

    private int _attackDamage;
    public int AttackDamage => _attackDamage;

    private float _speed;
    public float Speed => _speed;

    public void SetMonsterLevel(int level)
    {
        if (!_levelData.ContainsKey(level))
        {
            Debug.LogWarning($"Monster Level {level} is out of range. Using max level.");
            level = GetMaxLevel();
        }

        _level = level;
        ApplyLevelData();
    }
    
    private void ApplyLevelData()
    {
        MonsterLevelData data = _levelData[_level];

        _maxHealth = data.MaxHealth;
        _attackDamage = data.AttackDamage;
        _speed = data.Speed;
    }
    
    private int GetMaxLevel()
    {
        return _levelData.Count;
    }
}
