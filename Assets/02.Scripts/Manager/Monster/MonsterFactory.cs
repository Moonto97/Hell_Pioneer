using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFactory : MonoBehaviour
{
    public static MonsterFactory Instance {get; private set;}
    
    [SerializeField] private GameObject _monsterPrefab;
    [SerializeField] private int _poolSize = 100;
    
    private Queue<GameObject> _monsterPool = new Queue<GameObject>();

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        InitMonster();
    }

    private void InitMonster()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject monster = Instantiate(_monsterPrefab, transform);
            monster.SetActive(false);
            _monsterPool.Enqueue(monster);
        }
    }

    public GameObject MakeMonster(int monsterLevel, Vector3 position)
    {
        GameObject monster; 
        if (_monsterPool.Count > 0)
        {
            monster = _monsterPool.Dequeue();
        }
        else
        {
            // 풀에 없을 경우
            monster = Instantiate(_monsterPrefab, transform);
        }
        monster.transform.position = position;
        monster.SetActive(true);

        MonsterHealth monsterHealth = monster.GetComponent<MonsterHealth>();
        monsterHealth.OnMonsterDied += HandleMonsterDeath;
        
        SetMonsterState(monster, monsterLevel);
        
        return monster;
    }
    
    public void ReturnMonster(GameObject monster)
    {
        monster.SetActive(false);
        _monsterPool.Enqueue(monster);
    }

    private void SetMonsterState(GameObject monster, int monsterLevel)
    {
        MonsterHealth monsterHealth = monster.GetComponent<MonsterHealth>();
        MonsterStat monsterStat = monster.GetComponent<MonsterStat>();
        monsterStat.SetMonsterLevel(monsterLevel);
        monsterHealth.ResetHealth();
    }
    
    private void HandleMonsterDeath(MonsterHealth health)
    {
        health.OnMonsterDied -= HandleMonsterDeath;

        ReturnMonster(health.gameObject);
    }
}