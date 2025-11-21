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

    public GameObject MakeMonster()
    {
        if (_monsterPool.Count > 0)
        {
            GameObject monster = _monsterPool.Dequeue();
            monster.SetActive(true);
            return monster;
        }
        
        GameObject newMonster = Instantiate(_monsterPrefab, transform);
        return newMonster;
    }
    
    public void ReturnMonster(GameObject monster)
    {
        monster.SetActive(false);
        _monsterPool.Enqueue(monster);
    }
}