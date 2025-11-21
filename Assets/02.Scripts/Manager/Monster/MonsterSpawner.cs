using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("스폰 간격 설정")]
    public float MinSpawnInterval = 2f;
    public float MaxSpawnInterval = 4f;

    [Header("몬스터 레벨 가중치")] 
    public float Level1Weight = 50f;
    public float Level2Weight = 30f;
    public float Level3Weight = 20f;
    
    private float _spawnInterval;
    private float _timer;

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
        _spawnInterval = Random.Range(MinSpawnInterval, MaxSpawnInterval);
    }

    private void SpawnMonster()
    {
        GameObject monster = MonsterFactory.Instance.MakeMonster(GetRandomLevelWeighted(), transform.position);
    }
    
    private int GetRandomLevelWeighted()
    {
        float total = Level1Weight + Level2Weight + Level3Weight;
        float rand = Random.Range(0, total);

        if (rand < Level1Weight)
            return 1;

        rand -= Level1Weight;

        if (rand < Level2Weight)
            return 2;

        return 3;
    }
}