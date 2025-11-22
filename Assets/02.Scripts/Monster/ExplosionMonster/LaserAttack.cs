using System.Collections;
using UnityEngine;

public class LaserAttack : MonoBehaviour, IMonsterAttack
{
    private enum FireState
    {
        Idle,
        Aiming,
        Locked,
        Firing,
    }
    
    private Transform _player;
    private Transform _monster;
    
    private const string PlayerTag = "Player";
    private const string PlayerLayer = "Player";
    private const string WallLayer = "Wall";
    [Header("레이저 설정")] 
    [SerializeField] private float _firePointRadius = 0.5f;                // 몬스터 중심에서 떨어진 거리
    [SerializeField] private float _attackRange = 3f;               // 공격 가능 거리
    [SerializeField] private float _aimDuration = 2f;               // 조준 시간
    [SerializeField] private float _stopBeforeFireDuration = 1f;    // 조준 후 멈춤시간
    [SerializeField] private float _laserDuration = 0.5f;           // 레이저 지속 

    private FireState _fireState = FireState.Idle;
    private bool _isAttacking = false;
    private LineRenderer _laserRenderer;
    private MonsterFollow _monsterFollow;
    
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag(PlayerTag).transform;
        _monster = transform.parent;
        
        _laserRenderer = GetComponent<LineRenderer>();
        _laserRenderer.enabled = false;
        
        _monsterFollow = GetComponentInParent<MonsterFollow>();
    }

    private void Update()
    {
        if (_fireState == FireState.Aiming)
        {
            UpdateFirePosition();
        }
        
        float distance = Vector2.Distance(transform.position, _player.position);
        if (distance > _attackRange) return;

        if (_isAttacking) return;

        Attack();
    }

    private void UpdateFirePosition()
    {
        Vector2 direction = (_player.position - _monster.position).normalized;
        Vector3 offset = (Vector3)direction * _firePointRadius;

        transform.position = _monster.position + offset;
    }
    
    public void Attack()
    {
        StartCoroutine(LaserAttackRoutine());
    }

    private IEnumerator LaserAttackRoutine()
    {
        _isAttacking = true;
        if(_monsterFollow != null)
            _monsterFollow.enabled = false;
        
        // 조준
        _fireState = FireState.Aiming;
        
        float timer = 0f;
        while (timer < _aimDuration)
        {
            timer += Time.deltaTime;
            
            Vector2 direction = (_player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            
            _laserRenderer.enabled = true;
            _laserRenderer.startWidth = 0.05f;
            _laserRenderer.endWidth = 0.05f;
            _laserRenderer.SetPosition(0, transform.position);
            _laserRenderer.SetPosition(1, _player.position);
            yield return null;
        }
        
        // 조준 완료(정지)
        _fireState = FireState.Locked;
        Vector2 fireDirection = (_player.position - transform.position).normalized;
        float stopTimer = 0f;

        while (stopTimer < _stopBeforeFireDuration)
        {
            stopTimer += Time.deltaTime;

            Vector3 startPosition = transform.position;

            // 벽까지 레이저 체크
            RaycastHit2D wallHit = Physics2D.Raycast(
                startPosition,
                fireDirection,
                9999f,
                LayerMask.GetMask(WallLayer)
            );

            Vector3 wallPosition = wallHit.collider? (Vector3)wallHit.point
                : startPosition + (Vector3)fireDirection * 9999f;

            // 플레이어 체크 (벽까지 거리)
            float maxDist = Vector2.Distance(startPosition, wallPosition);

            var playerHit = Physics2D.Raycast(startPosition, fireDirection, maxDist, LayerMask.GetMask(PlayerLayer));

            Vector3 endPoint = playerHit.collider ? (Vector3)playerHit.point : wallPosition;

            _laserRenderer.startWidth = 0.1f;
            _laserRenderer.endWidth = 0.1f;

            _laserRenderer.SetPosition(0, startPosition);
            _laserRenderer.SetPosition(1, endPoint);

            yield return null;
        }

        
        // 레이저 발사
        _fireState = FireState.Firing;
        _laserRenderer.startWidth = 0.2f;
        _laserRenderer.endWidth = 0.2f;

        bool hasDamaged = false;
        Vector3 fireStart = transform.position;
        
        RaycastHit2D finalWallHit = Physics2D.Raycast(
            fireStart,
            fireDirection,
            9999f,
            LayerMask.GetMask(WallLayer)
        );
        
        Vector3 wallEnd = finalWallHit.collider ? (Vector3)finalWallHit.point
            : fireStart + (Vector3)fireDirection * 9999f;
        
        // 플레이어 히트
        RaycastHit2D finalPlayerHit = Physics2D.Raycast(
            fireStart,
            fireDirection,
            Vector2.Distance(fireStart, wallEnd),
            LayerMask.GetMask(PlayerLayer)
        );

        Vector3 finalLaserEnd = wallEnd;
        
        if (finalPlayerHit.collider)
        {
            finalLaserEnd = finalPlayerHit.point;
            if (!hasDamaged)
            {
                hasDamaged = true;
                Debug.Log("Hit");
                // TODO: 플레이어 데미지 주기
            }
        }
        
        // 최종 레이저 라인
        _laserRenderer.SetPosition(0, fireStart);
        _laserRenderer.SetPosition(1, finalLaserEnd);

        yield return new WaitForSeconds(_laserDuration);
        
        _laserRenderer.enabled = false;
        if(_monsterFollow != null)
            _monsterFollow.enabled = true;
        _isAttacking = false;
        _fireState = FireState.Idle;
    }
}
