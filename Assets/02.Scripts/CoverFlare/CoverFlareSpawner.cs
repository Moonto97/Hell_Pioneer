using UnityEngine;

/// <summary>
/// 플레이어가 우클릭했을 때 벽(커버)을 생성하는 컴포넌트.
/// - 입력 처리
/// - 위치 계산 (마우스 위치, 최대 사거리)
/// - 설치 가능 여부 체크
/// - 차지/재충전 관리
/// - CoverWall 프리팹 생성
/// </summary>
public class CoverFlareSpawner : MonoBehaviour
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


// --- 런타임 상태 ---
// 현재 상태를 저장하고, 외부에서 읽을 수 있도록 노출하는 부분
private int _currentCharges;
    private float _rechargeTimer;

    public int CurrentCharges => _currentCharges;
    public int MaxCharges => _maxCharges;
    public bool HasAnyCharge => _currentCharges > 0;

    private void Awake()
    {
        // 카메라 레퍼런스가 비어있으면 메인 카메라 사용
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        // 시작할 때 차지를 꽉 채운 상태로 시작
        _currentCharges = _maxCharges;
        _rechargeTimer = 0f;
    }

    private void Update()
    {
        HandleInput();
        HandleRecharge(Time.deltaTime);
    }

    /// <summary>
    /// 우클릭 입력을 처리합니다.
    /// </summary>
    private void HandleInput()
    {
        // 마우스 오른쪽 버튼 Down
        if (Input.GetMouseButtonDown(1))
        {
            TrySpawnCoverAtMouse();
        }
    }

    /// <summary>
    /// 매 프레임 차지 재충전을 처리합니다.
    /// </summary>
    private void HandleRecharge(float deltaTime)
    {
        // 이미 최대 차지면 타이머 돌릴 필요 없음
        if (_currentCharges >= _maxCharges) return;
        if (_rechargeTimePerCharge <= 0f) return;

        _rechargeTimer -= deltaTime;

        if (_rechargeTimer <= 0f)
        {
            _currentCharges++;

            // 아직 최대가 아니면 다시 타이머 세팅
            if (_currentCharges < _maxCharges)
            {
                _rechargeTimer = _rechargeTimePerCharge;
            }
            else
            {
                _rechargeTimer = 0f;
            }
        }
    }

    /// <summary>
    /// 마우스 위치 기준으로 벽 생성 시도.
    /// </summary>
    private void TrySpawnCoverAtMouse()
    {
        if (_coverPrefab == null || _camera == null)
            return;

        if (!HasAnyCharge)
        {
            // TODO: UI로 "차지가 없습니다" 같은 피드백 주기
            return;
        }

        // 1) 마우스 화면 좌표 → 월드 좌표로 변환
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = _camera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;  // 2D라서 z는 0 고정

        // 2) 플레이어 위치 기준 최대 사거리 제한
        Vector3 playerPos = transform.position;
        Vector3 direction = mouseWorldPos - playerPos;
        float distance = direction.magnitude;

        if (distance > _maxSpawnDistance)
        {
            direction.Normalize();
            mouseWorldPos = playerPos + direction * _maxSpawnDistance;
        }

        // 3) 해당 위치에 설치 가능한지(충돌 체크)
        if (CanPlaceCover(mouseWorldPos) == false)
        {
            // TODO: "설치 불가" 이펙트나 사운드
            return;
        }

        // 4) 실제로 차지 소모
        if (ConsumeCharge() == false)
        {
            return;
        }

        // 5) 프리팹 생성
        GameObject coverObj = Instantiate(_coverPrefab, mouseWorldPos + (Vector3)_spawnOffset, Quaternion.identity);

        // 6) 생성된 벽에 HP/수명 세팅
        if (coverObj.TryGetComponent(out CoverWall coverWall))
        {
            coverWall.Initialize(_defaultCoverHP, _defaultCoverLifetime);
        }
    }

    /// <summary>
    /// 벽 설치 가능 여부를 검사합니다.
    /// - 지정한 반경 내에 blockedLayers가 있으면 설치 불가
    /// </summary>
    private bool CanPlaceCover(Vector3 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, _placementCheckRadius, _blockedLayers);
        return hit == null;
    }

    /// <summary>
    /// 차지를 1개 소모합니다.
    /// </summary>
    private bool ConsumeCharge()
    {
        if (HasAnyCharge == false) return false;

        _currentCharges--;

        // 처음 차지를 소모하면 재충전 타이머 시작
        if (_currentCharges < _maxCharges && _rechargeTimer <= 0f)
        {
            _rechargeTimer = _rechargeTimePerCharge;
        }

        return true;
    }

#if UNITY_EDITOR
    // 에디터에서 플레이어 주변에 최대 사거리 / 설치 체크 반경을 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _maxSpawnDistance);
    }
#endif
}