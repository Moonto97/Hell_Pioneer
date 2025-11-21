using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public float MinSpawnInterval = 2f;
    public float MaxSpawnInterval = 4f;
    
    private float _spawnInterval;
    private float _timer;

    private void Start()
    {
        SetRandomInterval();
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _spawnInterval)
        {
            _timer = 0;
            SpawnMonster();
            SetRandomInterval();
        }
    }

    private void SetRandomInterval()
    {
        _spawnInterval = Random.Range(MinSpawnInterval, MaxSpawnInterval);
    }

    private void SpawnMonster()
    {
        GameObject monster = MonsterFactory.Instance.MakeMonster();
        monster.transform.position = transform.position;
    }
}