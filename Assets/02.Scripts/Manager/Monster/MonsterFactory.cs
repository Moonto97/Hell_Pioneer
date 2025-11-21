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
        if (_monsterPool.Count > 0)
        {
            GameObject monster = _monsterPool.Dequeue();
            monster.SetActive(true);
            MonsterHealth monsterHealth = monster.GetComponent<MonsterHealth>();
            monsterHealth.OnMonsterDied += HandleMonsterDeath;
            monsterHealth.ResetHealth();
            SetMonsterState(monster, monsterLevel);
            return monster;
        }
        
        // 풀에 없을 경우
        GameObject newMonster = Instantiate(_monsterPrefab, transform);
        newMonster.SetActive(true);
        MonsterHealth newMonsterHealth = newMonster.GetComponent<MonsterHealth>();
        newMonsterHealth.OnMonsterDied += HandleMonsterDeath;
        newMonsterHealth.ResetHealth();
        SetMonsterState(newMonster, monsterLevel);
        return newMonster;
    }
    
    public void ReturnMonster(GameObject monster)
    {
        monster.SetActive(false);
        _monsterPool.Enqueue(monster);
    }

    private void SetMonsterState(GameObject monster, int monsterLevel)
    {
        MonsterStat monsterStat = monster.GetComponent<MonsterStat>();
        MonsterHealth monsterHealth = monster.GetComponent<MonsterHealth>();
        monsterStat.SetMonsterLevel(monsterLevel);
        monsterHealth.ResetHealth();
    }
    
    private void HandleMonsterDeath(MonsterHealth health)
    {
        health.OnMonsterDied -= HandleMonsterDeath;

        ReturnMonster(health.gameObject);
    }
}