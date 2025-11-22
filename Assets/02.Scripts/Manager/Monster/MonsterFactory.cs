using System;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType
{
    Explosion,
    Laser
}

public class MonsterFactory : MonoBehaviour
{
    public static MonsterFactory Instance {get; private set;}
    [System.Serializable]
    private class MonsterPoolInfo
    {
        public MonsterType Type;
        public GameObject Prefab;
        public int InitialSize;
    }
    
    [SerializeField] private List<MonsterPoolInfo> monsterPoolInfos;
    
    private Dictionary<MonsterType, Queue<GameObject>> _pools = new ();
    private Dictionary<MonsterType, GameObject> _prefabTable = new ();
    
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        InitPools();
    }

    private void InitPools()
    {
        foreach (MonsterPoolInfo info in monsterPoolInfos)
        {
            _pools[info.Type] = new Queue<GameObject>();
            _prefabTable[info.Type] = info.Prefab;

            for (int i = 0; i < info.InitialSize; i++)
            {
                GameObject monster = Instantiate(info.Prefab, transform);
                monster.SetActive(false);
                _pools[info.Type].Enqueue(monster);
            }
        }
    }

    public GameObject MakeMonster(MonsterType monsterType, int monsterLevel, Vector3 position)
    {
        GameObject monster = GetFromPool(monsterType);
        monster.transform.position = position;
        monster.SetActive(true);

        // Health 이벤트 등록
        MonsterHealth health = monster.GetComponent<MonsterHealth>();
        health.OnMonsterDied += HandleMonsterDeath;

        // 스탯 초기화
        SetMonsterState(monster, monsterType, monsterLevel);
        
        return monster;
    }
    
    private GameObject GetFromPool(MonsterType type)
    {
        if (_pools[type].Count > 0)
        {
            return _pools[type].Dequeue();
        }

        GameObject newMonster = Instantiate(_prefabTable[type], transform);
        return newMonster;
    }
    
    private void ReturnMonster(MonsterType type, GameObject monster)
    {
        monster.SetActive(false);
        _pools[type].Enqueue(monster);
    }
    

    private void SetMonsterState(GameObject monster, MonsterType type, int monsterLevel)
    {
        MonsterHealth monsterHealth = monster.GetComponent<MonsterHealth>();
        MonsterStat monsterStat = monster.GetComponent<MonsterStat>();
        monsterStat.SetMonsterType(type);  
        monsterStat.SetMonsterLevel(monsterLevel);
        monsterHealth.ResetHealth();
    }
    
    private void HandleMonsterDeath(MonsterHealth health)
    {
        health.OnMonsterDied -= HandleMonsterDeath;

        MonsterType type = health.GetComponent<MonsterStat>().MonsterType;

        ReturnMonster(type, health.gameObject);
    }
}