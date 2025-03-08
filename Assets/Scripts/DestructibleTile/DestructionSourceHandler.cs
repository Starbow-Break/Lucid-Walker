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
            Vector2 impactPosition = transform.position;

            Vector2 direction = GetDirectionFromImpactPosition(impactPosition);
            tilesToBreak = GetAffectedTiles(impactPosition, defaultRadius, direction);
            Debug.Log($"[Trigger] Tiles to break count: {tilesToBreak.Count}");
        }
    }

    private void FixedUpdate()
    {
        if (!isTriggered || tilesToBreak.Count == 0) return;

        if (tilesToBreak.Count > 0)
        {
            if (updateTimer < maxTime) updateTimer++;
            else
            {
                List<Vector3Int> tilesToRemoveFromList = new List<Vector3Int>();
                List<DestructionVisualHandler> convertedTiles = new List<DestructionVisualHandler>();

                foreach (var tile in tilesToBreak)
                {
                    Vector3Int position = tile;
                    position.z = (int)destructibleTileMap.transform.position.z;

                    if (destructibleTileMap.GetTile(position))
                    {
                        if (!tilesToRemoveFromList.Contains(position))
                        {
                            // 파괴될 타일 위치 확인
                            Debug.Log($"[FixedUpdate] Destroying tile at: {position}");

                            // 풀링된 오브젝트(또는 Instantiate) 생성
                            GameObject newTile = ObjectPooler.Instance.GetPooledObject(tileSplitterPrefab, position, Quaternion.identity);

                            DestructionVisualHandler handler = newTile.GetComponent<DestructionVisualHandler>();
                            handler.tileSprite = destructibleTileMap.GetSprite(position);
                            handler.forcePosition = transform.position;

                            convertedTiles.Add(handler);

                            // 원본 타일 제거
                            destructibleTileMap.SetTile(position, null);
                            tilesToRemoveFromList.Add(position);
                        }
                    }
                    else
                    {
                        // 이미 타일이 없으면 리스트에서 제거
                        tilesToRemoveFromList.Add(position);
                    }
                }
                foreach (var tile in tilesToRemoveFromList)
                {
                    tilesToBreak.Remove(tile);
                }
                tilesToRemoveFromList.Clear();
                updateTimer = 0;
            }
        }
        else
        {
            // 파괴 대상이 더 이상 없으면, 풀링 혹은 오브젝트 제거
            ObjectPooler.Instance.ReturnPooledObject(this.gameObject);
            // Destroy(gameObject);
        }
    }

    private List<Vector3Int> GetAffectedTiles(Vector2 impactPosition, Vector2Int impactRadius, Vector2 direction)
    {
        if (impactRadius == new Vector2Int(0, 0))
            impactRadius = defaultRadius;

        List<Vector3Int> allTiles = new List<Vector3Int>();
        Vector3 rightDirection = Quaternion.Euler(0, 0, -90f) * direction;
        Vector3Int startPosition = AdjustStartPosition(impactPosition, rightDirection, direction);

        if (destructibleTileMap.GetTile(startPosition))
        {
            List<Vector3Int> startTiles = new List<Vector3Int>();
            int width = impactRadius.x;
            if (impactRadius.x > 1) impactRadius.x = (width - 1) / 2;

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

                int multiplyer = (direction.y > 0) ? 1 : -1;
                foreach (var point in startTiles)
                {
                    allTiles.Add(point);
                    for (int y = 0; y < impactRadius.y; y++)
                    {
                        AddTile(allTiles, point + new Vector3Int(0, y * multiplyer, 0));
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

                int multiplyer = (direction.x > 0) ? 1 : -1;
                foreach (var point in startTiles)
                {
                    allTiles.Add(point);
                    for (int x = 0; x < impactRadius.y; x++)
                    {
                        AddTile(allTiles, point + new Vector3Int(x * multiplyer, 0, 0));
                    }
                }
            }
        }
        return allTiles;
    }

    private void AddTile(List<Vector3Int> list, Vector3Int pos)
    {
        if (destructibleTileMap.GetTile(pos) && !list.Contains(pos))
            list.Add(pos);
    }

    private Vector3Int AdjustStartPosition(Vector2 impactPosition, Vector3 rightDirection, Vector2 direction)
    {
        Vector3Int position = new Vector3Int();

        if (direction.x == 0) // UP/DOWN
        {
            int offsetVal = direction.y > 0 ? 1 : -2;
            int xOffset = direction.y > 0 ? -1 : 0;
            position = new Vector3Int((int)(impactPosition.x + rightDirection.x + xOffset), (int)(impactPosition.y + offsetVal), 0);
        }
        else if (direction.y == 0) // LEFT/RIGHT
        {
            int offsetVal = direction.x > 0 ? 1 : -2;
            int yOffset = direction.x > 0 ? 0 : -1;
            position = new Vector3Int((int)(impactPosition.x + offsetVal), (int)(impactPosition.y + rightDirection.y + yOffset), 0);
        }

        return position;
    }

    private Vector2 GetDirectionFromImpactPosition(Vector2 impactPosition)
    {
        Vector2 direction = Vector2.zero;
        Vector3Int position = new Vector3Int((int)(impactPosition.x + offset.x), (int)(impactPosition.y + offset.y), 0);

        if (destructibleTileMap.GetTile(position + Vector3Int.right))
            direction.x = 1;
        else if (destructibleTileMap.GetTile(position + Vector3Int.left * 2))
            direction.x = -1;
        else if (destructibleTileMap.GetTile(position + Vector3Int.up))
            direction.y = 1;
        else if (destructibleTileMap.GetTile(position + Vector3Int.down * 2))
            direction.y = -1;

        return direction;
    }
}
