using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructionSourceHandler : MonoBehaviour
{
    public int updateTimer;
    public int maxTime;
    public Vector2 offset;
    [SerializeField] private Vector2Int defaultRadius;
    [SerializeField] private Tilemap destructibleTileMap;
    [SerializeField] private GameObject tileSplitterPrefab;

    private List<Vector3Int> tilesToBreak = new List<Vector3Int>();
    private bool isTriggered = false;

    private void Start()
    {
        if (destructibleTileMap == null)
            destructibleTileMap = GameObject.Find("DestructibleTileMap").GetComponent<Tilemap>();
    }

    public void TriggerDestruction()
    {
        if (!isTriggered)
        {
            isTriggered = true;
            // 월드 좌표 그대로 사용. 이후 셀 좌표 변환을 각 함수 내에서 처리함
            Vector2 impactPosition = transform.position;

            // GetDirectionFromImpactPosition()에서 월드→셀 좌표 변환을 진행
            Vector2 direction = GetDirectionFromImpactPosition(impactPosition);
            // GetAffectedTiles()에서도 내부적으로 셀 좌표를 기준으로 계산함
            tilesToBreak = GetAffectedTiles(impactPosition, defaultRadius, direction);
            Debug.Log($"[Trigger] Tiles to break count: {tilesToBreak.Count}");
        }
    }

    private void FixedUpdate()
    {
        if (!isTriggered || tilesToBreak.Count == 0) return;

        if (tilesToBreak.Count > 0)
        {
            if (updateTimer < maxTime)
                updateTimer++;
            else
            {
                List<Vector3Int> tilesToRemoveFromList = new List<Vector3Int>();
                List<DestructionVisualHandler> convertedTiles = new List<DestructionVisualHandler>();

                foreach (var tile in tilesToBreak)
                {
                    Vector3Int cellPos = tile;
                    // z값은 타일맵 기준으로 보통 0이므로 따로 처리 (필요시 수정)
                    cellPos.z = 0;

                    if (destructibleTileMap.GetTile(cellPos))
                    {
                        if (!tilesToRemoveFromList.Contains(cellPos))
                        {
                            Debug.Log($"[FixedUpdate] Destroying tile at cell: {cellPos}");

                            // 셀 좌표를 월드 좌표로 변환하여 오브젝트 생성
                            Vector3 worldPos = destructibleTileMap.CellToWorld(cellPos);
                            GameObject newTile = ObjectPooler.Instance.GetPooledObject(tileSplitterPrefab, worldPos, Quaternion.identity);

                            DestructionVisualHandler handler = newTile.GetComponent<DestructionVisualHandler>();
                            handler.tileSprite = destructibleTileMap.GetSprite(cellPos);
                            handler.forcePosition = transform.position;

                            convertedTiles.Add(handler);

                            // 타일 제거
                            destructibleTileMap.SetTile(cellPos, null);
                            tilesToRemoveFromList.Add(cellPos);
                        }
                    }
                    else
                    {
                        tilesToRemoveFromList.Add(cellPos);
                    }
                }
                foreach (var cell in tilesToRemoveFromList)
                {
                    tilesToBreak.Remove(cell);
                }
                tilesToRemoveFromList.Clear();
                updateTimer = 0;
            }
        }
        else
        {
            ObjectPooler.Instance.ReturnPooledObject(this.gameObject);
            // Destroy(gameObject);
        }
    }

    private List<Vector3Int> GetAffectedTiles(Vector2 impactPosition, Vector2Int impactRadius, Vector2 direction)
    {
        if (impactRadius == new Vector2Int(0, 0))
            impactRadius = defaultRadius;

        List<Vector3Int> allTiles = new List<Vector3Int>();
        // rightDirection은 그대로 사용 (방향 계산에 필요)
        Vector3 rightDirection = Quaternion.Euler(0, 0, -90f) * direction;
        // impactPosition을 월드 좌표에서 셀 좌표로 변환 후, 오프셋을 적용하여 시작 셀을 계산
        Vector3Int startPosition = AdjustStartPosition(impactPosition, rightDirection, direction);
        Debug.Log($"[GetAffectedTiles] Start Cell: {startPosition}");

        if (destructibleTileMap.GetTile(startPosition))
        {
            List<Vector3Int> startTiles = new List<Vector3Int>();
            int width = impactRadius.x;
            if (impactRadius.x > 1)
                impactRadius.x = (width - 1) / 2;

            if (direction.x == 0) // UP & DOWN
            {
                if (width > 1)
                {
                    for (int x = -impactRadius.x; x < impactRadius.x; x++)
                    {
                        AddTile(startTiles, startPosition + new Vector3Int(x, 0, 0));
                        AddTile(startTiles, startPosition - new Vector3Int(x, 0, 0));
                    }
                }
                else
                {
                    AddTile(startTiles, startPosition);
                }

                int multiplier = (direction.y > 0) ? 1 : -1;
                foreach (var point in startTiles)
                {
                    allTiles.Add(point);
                    for (int y = 0; y < impactRadius.y; y++)
                    {
                        AddTile(allTiles, point + new Vector3Int(0, y * multiplier, 0));
                    }
                }
            }
            else if (direction.y == 0) // LEFT & RIGHT
            {
                if (width > 1)
                {
                    for (int y = -impactRadius.x; y < impactRadius.x; y++)
                    {
                        AddTile(startTiles, startPosition + new Vector3Int(0, y, 0));
                        AddTile(startTiles, startPosition - new Vector3Int(0, y, 0));
                    }
                }
                else
                {
                    AddTile(startTiles, startPosition);
                }

                int multiplier = (direction.x > 0) ? 1 : -1;
                foreach (var point in startTiles)
                {
                    allTiles.Add(point);
                    for (int x = 0; x < impactRadius.y; x++)
                    {
                        AddTile(allTiles, point + new Vector3Int(x * multiplier, 0, 0));
                    }
                }
            }
        }
        return allTiles;
    }

    private void AddTile(List<Vector3Int> list, Vector3Int cellPos)
    {
        if (destructibleTileMap.GetTile(cellPos) && !list.Contains(cellPos))
            list.Add(cellPos);
    }

    // 월드 좌표를 셀 좌표로 변환한 후, 추가 오프셋을 더해서 시작 셀을 계산
    private Vector3Int AdjustStartPosition(Vector2 impactPosition, Vector3 rightDirection, Vector2 direction)
    {
        // impactPosition을 셀 좌표로 변환
        Vector3Int baseCell = destructibleTileMap.WorldToCell(impactPosition);
        Vector3Int adjustedCell = baseCell;

        if (direction.x == 0) // UP/DOWN
        {
            int offsetVal = direction.y > 0 ? 1 : -2;
            int xOffset = direction.y > 0 ? -1 : 0;
            adjustedCell += new Vector3Int(xOffset, offsetVal, 0);
        }
        else if (direction.y == 0) // LEFT/RIGHT
        {
            int offsetVal = direction.x > 0 ? 1 : -2;
            int yOffset = direction.x > 0 ? 0 : -1;
            adjustedCell += new Vector3Int(offsetVal, yOffset, 0);
        }

        return adjustedCell;
    }

    // 월드 좌표에 offset을 적용한 후, 셀 좌표로 변환하여 인접 타일을 확인
    private Vector2 GetDirectionFromImpactPosition(Vector2 impactPosition)
    {
        Vector2 direction = Vector2.zero;
        Vector3 impactPosWithOffset = impactPosition + offset;
        Vector3Int cellPosition = destructibleTileMap.WorldToCell(impactPosWithOffset);
        Debug.Log($"[GetDirectionFromImpactPosition] Checking cell: {cellPosition}");

        if (destructibleTileMap.GetTile(cellPosition + Vector3Int.right))
            direction.x = 1;
        else if (destructibleTileMap.GetTile(cellPosition + Vector3Int.left * 2))
            direction.x = -1;
        else if (destructibleTileMap.GetTile(cellPosition + Vector3Int.up))
            direction.y = 1;
        else if (destructibleTileMap.GetTile(cellPosition + Vector3Int.down * 2))
            direction.y = -1;

        Debug.Log($"[GetDirectionFromImpactPosition] Resulting direction: {direction}");
        return direction;
    }
}
