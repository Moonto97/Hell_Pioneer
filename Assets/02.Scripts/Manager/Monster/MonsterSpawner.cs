using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("스폰 간격 설정")]
    [SerializeField] private float _minSpawnInterval = 2f;
    [SerializeField] private float _maxSpawnInterval = 4f;

    [Header("몬스터 레벨 가중치")] 
    [SerializeField] private float _level1Weight = 50f;
    [SerializeField] private float _level2Weight = 30f;
    [SerializeField] private float _level3Weight = 20f;
    
    [Header("몬스터 종류")]
    [SerializeField] private MonsterType _monsterType = MonsterType.Explosion;
    
    private float _spawnInterval;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true) // 게임 로직에 따라 종료 조건 추가 가능
        {
            SetRandomInterval();
            yield return new WaitForSeconds(_spawnInterval);
            SpawnMonster();
        }
    }

    private void SetRandomInterval()
    {
        _spawnInterval = Random.Range(_minSpawnInterval, _maxSpawnInterval);
    }

    private void SpawnMonster()
    {
        GameObject monster = MonsterFactory.Instance.MakeMonster(_monsterType, GetRandomLevelWeighted(), transform.position);
    }
    
    private int GetRandomLevelWeighted()
    {
        float total = _level1Weight + _level2Weight + _level3Weight;
        float rand = Random.Range(0, total);

        if (rand < _level1Weight)
            return 1;

        rand -= _level1Weight;

        if (rand < _level2Weight)
            return 2;

        return 3;
    }
}