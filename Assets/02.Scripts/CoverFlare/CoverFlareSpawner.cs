using UnityEngine;

// 플레이어가 우클릭했을 때 벽(커버)을 생성하는 컴포넌트.
// - 입력 처리
// - 위치 계산 (마우스 위치, 최대 사거리)
// - 설치 가능 여부 체크
// - 차지/재충전 관리
// - CoverWall 프리팹 생성
public class CoverFlareSpawner : MonoBehaviour, ICoverFlareOwner
{
    [Header("References")]
    [SerializeField] private GameObject _coverPrefab;  // 생성할 벽 프리팹
    [SerializeField] private Camera _camera;           // 비워두면 Camera.main 사용

    [Header("Spawn Settings")]
    [Tooltip("플레이어 기준 최대 생성 거리")]
    [SerializeField] private float _maxSpawnDistance = 3f;

    [Tooltip("벽을 설치할 수 없는 레이어 (지형, 다른 벽 등)")]
    [SerializeField] private LayerMask _blockedLayers;

    [Tooltip("설치 가능 여부를 확인할 때 사용하는 원의 반지름")]
    [SerializeField] private float _placementCheckRadius = 0.25f;

    [Header("Charge Settings")]
    [Tooltip("동시에 보유할 수 있는 최대 차지 수")]
    [SerializeField] private int _maxCharges = 3;

    [Tooltip("차지 1개가 재충전되는 데 걸리는 시간(초)")]
    [SerializeField] private float _rechargeTimePerCharge = 5f;

    [Header("Cover Defaults")]
    [Tooltip("생성되는 벽의 기본 체력")]
    [SerializeField] private int _defaultCoverHP = 3;

    [Tooltip("생성되는 벽의 기본 유지 시간(초)")]
    [SerializeField] private float _defaultCoverLifetime = 4f;
        
    [SerializeField] private Vector2 _spawnOffset;  // 벽 생성 위치 오프셋
    private const int MouseRightButton = 1;       // 우클릭 버튼 번호

    // --- 런타임 상태 ---
    private int _currentCharges;
    private float _rechargeTimer;

    public int CurrentCharges => _currentCharges;
    public int MaxCharges => _maxCharges;
    public bool HasAnyCharge => _currentCharges > 0;

    private void Awake()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        _currentCharges = _maxCharges;
        _rechargeTimer = 0f;
    }

    private void Update()
    {
        HandleInput();
        HandleRecharge(Time.deltaTime);
    }

    // 우클릭 입력을 처리합니다
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(MouseRightButton))
        {
            TrySpawnCoverAtMouse();
        }
    }

    // 매 프레임 차지 재충전을 처리합니다
    private void HandleRecharge(float deltaTime)
    {
        if (_currentCharges >= _maxCharges) return;
        if (_rechargeTimePerCharge <= 0f) return;

        _rechargeTimer -= deltaTime;

        if (_rechargeTimer <= 0f)
        {
            _currentCharges++;
            _rechargeTimer = _currentCharges < _maxCharges ? _rechargeTimePerCharge : 0f;
        }
    }

    // 마우스 위치 기준으로 벽 생성 시도 (오케스트레이션)
    private void TrySpawnCoverAtMouse()
    {
        if (!CanSpawn())
        {
            // TODO: UI 피드백 (차지 부족 / 프리팹 누락 / 카메라 없음)
            return;
        }

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector3 clampedPos = ClampToMaxDistance(mouseWorldPos);

        if (!IsValidSpawnPosition(clampedPos))
        {
            // TODO: 설치 불가 이펙트/사운드
            return;
        }

        if (!ConsumeCharge())
        {
            return;
        }

        GameObject coverObj = CreateCover(clampedPos);
        InitializeCoverIfPossible(coverObj);
    }

    // 스폰 가능한 기본 조건 검사
    private bool CanSpawn()
    {
        if (_coverPrefab == null) return false;
        if (_camera == null) return false;
        if (!HasAnyCharge) return false;
        return true;
    }

    /// 마우스 화면 좌표를 월드 좌표(2D z=0)로 변환
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 screen = Input.mousePosition;
        Vector3 world = _camera.ScreenToWorldPoint(screen);
        world.z = 0f;
        return world;
    }

    /// 플레이어 기준 최대 사거리 내로 위치를 클램프
    private Vector3 ClampToMaxDistance(Vector3 targetWorldPos)
    {
        Vector3 playerPos = transform.position;
        Vector3 direction = targetWorldPos - playerPos;
        float distance = direction.magnitude;

        if (distance > _maxSpawnDistance)
        {
            direction.Normalize();
            return playerPos + direction * _maxSpawnDistance;
        }

        return targetWorldPos;
    }

    /// 해당 위치가 설치 가능(충돌 없음)한지 검사
    private bool IsValidSpawnPosition(Vector3 position)
    {
        return CanPlaceCover(position);
    }

    // 커버 프리팹을 생성
    private GameObject CreateCover(Vector3 position)
    {
        return Instantiate(_coverPrefab, position + (Vector3)_spawnOffset, Quaternion.identity);
    }

    // 커버월 컴포넌트 초기화 시도
    private void InitializeCoverIfPossible(GameObject coverObj)
    {
        if (coverObj != null && coverObj.TryGetComponent(out CoverWall coverWall))
        {
            coverWall.Initialize(_defaultCoverHP, _defaultCoverLifetime);
        }
    }

    // 벽 설치 가능 여부를 검사합니다.
    // - 지정한 반경 내에 blockedLayers가 있으면 설치 불가
    private bool CanPlaceCover(Vector3 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, _placementCheckRadius, _blockedLayers);
        return hit == null;
    }

    // 차지 1개 소모
    private bool ConsumeCharge()
    {
        if (!HasAnyCharge) return false;

        _currentCharges--;

        if (_currentCharges < _maxCharges && _rechargeTimer <= 0f)
        {
            _rechargeTimer = _rechargeTimePerCharge;
        }

        return true;
    }

    // 커버플레어의 최대 차지 수(탄창 용량)를 증가시킵니다.
    public void IncreaseCoverFlareMaxCharges(int amount)
    {
        _maxCharges += amount;
        _currentCharges += amount;

        if (_currentCharges > _maxCharges)
        {
            _currentCharges = _maxCharges;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _maxSpawnDistance);
    }
#endif
}