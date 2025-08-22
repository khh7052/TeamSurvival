using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class StartupConsumableDropper : MonoBehaviour
{
    [Header("드랍 허용 아이디(화이트리스트) — 반드시 5개만")]
    [SerializeField] private List<int> allowedIds = new List<int>();

    [System.Serializable]
    public class DropEntry
    {
        public int itemId;
        public int amount = 1;
    }

    [Header("드랍 목록(화이트리스트에 포함된 것만 드랍됨)")]
    [SerializeField] private List<DropEntry> drops = new List<DropEntry>();

    [Header("드랍 영역(선택) — 지정 시, 이들 콜라이더 내부에 랜덤 드랍")]
    [SerializeField] private Collider[] dropAreas;

    [Header("바닥 탐지")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float dropHeight = 10f;
    [SerializeField] private float heightOffsetDown = 0.1f;

    [Header("충돌/간격")]
    [SerializeField] private float minDistanceBetweenDrops = 2f;
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private string interactableLayerName = "Interactable";

    [Header("범위/시도 설정")]
    [SerializeField] private float dropRadius = 25f;      // dropAreas 없을 때만 사용
    [SerializeField] private int maxTriesPerDrop = 60;    // 아이템 1개당 최대 시도
    [SerializeField] private float backoffRatio = 0.75f;  // 실패 시 간격 완화 비율

    [Header("초기화 대기")]
    [SerializeField] private bool waitForGameManager = true;

    private bool _done;
    private List<Vector3> spawnPositions = new List<Vector3>();

    private async void Start()
    {
        if (_done) return;
        if (waitForGameManager)
        {
            while (GameManager.Instance == null || !GameManager.Instance.IsInitialized)
                await Task.Yield();
        }

        if (allowedIds == null || allowedIds.Count != 5)
        {
            Debug.LogWarning("[StartupDropper] allowedIds 는 정확히 5개여야 합니다.");
        }

        await DropAll();
        _done = true;
    }

    private async Task DropAll()
    {
        spawnPositions.Clear();
        int spawnedTotal = 0;

        foreach (var entry in drops)
        {
            if (allowedIds == null || !allowedIds.Contains(entry.itemId))
                continue;

            int count = Mathf.Max(1, entry.amount);
            for (int i = 0; i < count; i++)
            {
                float curMinDist = minDistanceBetweenDrops;
                float curRadius = dropRadius; // dropAreas 없을 때만 사용됨
                bool placed = false;

                for (int tries = 0; tries < maxTriesPerDrop && !placed; tries++)
                {
                    // 실패 누적될수록 완화 (예: 1/3, 2/3 지점에서 완화)
                    float t = (tries + 1f) / maxTriesPerDrop;
                    if (t > 0.33f) curMinDist = minDistanceBetweenDrops * backoffRatio;
                    if (t > 0.66f)
                    {
                        curMinDist = minDistanceBetweenDrops * backoffRatio * backoffRatio;
                        curRadius = dropRadius / backoffRatio;
                    }

                    Vector3 spawnPos;

                    // 영역 지정된 경우
                    if (dropAreas != null && dropAreas.Length > 0)
                    {
                        if (!TryGetDropPosition(out spawnPos)) continue;
                    }
                    else
                    {
                        // 반경 기반
                        Vector2 r = Random.insideUnitCircle * curRadius;
                        Vector3 start = new Vector3(transform.position.x + r.x, dropHeight, transform.position.z + r.y);
                        if (!Physics.Raycast(start, Vector3.down, out RaycastHit hit, dropHeight * 2f, groundLayer))
                            continue;
                        spawnPos = hit.point + Vector3.down * heightOffsetDown;
                    }

                    // 간격/충돌 체크 (완화된 curMinDist 사용)
                    if (spawnPositions.Any(p => Vector3.Distance(p, spawnPos) < curMinDist)) continue;
                    if (Physics.CheckSphere(spawnPos, curMinDist, interactableLayerMask)) continue;

                    // 예약 및 생성
                    spawnPositions.Add(spawnPos);
                    AssetDataLoader.Instance.InstantiateByID(entry.itemId, go =>
                    {
                        go.transform.SetPositionAndRotation(spawnPos, Quaternion.identity);
                        int targetLayer = LayerMask.NameToLayer(interactableLayerName);
                        if (targetLayer >= 0) SetLayerRecursively(go, targetLayer);
                    });

                    spawnedTotal++;
                    placed = true;
                    await Task.Yield();
                }

                if (!placed)
                    Debug.LogWarning($"[StartupDropper] {entry.itemId} 자리 부족으로 스폰 실패(요청 {count} 중 {i + 1}번째).");
            }
        }

        Debug.Log($"[StartupDropper] 스폰 완료: 요청={drops.Sum(d => Mathf.Max(1, d.amount))}, 실제={spawnedTotal}");
    }

    private bool TryGetDropPosition(out Vector3 pos)
    {
        if (dropAreas != null && dropAreas.Length > 0)
        {
            for (int tries = 0; tries < 20; tries++)
            {
                var area = dropAreas[Random.Range(0, dropAreas.Length)];
                var b = area.bounds;
                var start = new Vector3(
                    Random.Range(b.min.x, b.max.x),
                    b.max.y + dropHeight,
                    Random.Range(b.min.z, b.max.z)
                );

                if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, dropHeight * 2f, groundLayer))
                {
                    pos = hit.point + Vector3.down * heightOffsetDown;
                    return true;
                }
            }
        }

        pos = Vector3.zero;
        return false;
    }

    private void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
