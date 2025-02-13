using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ScriptableTilemapCollider : MonoBehaviour
{
    [SerializeField] Tilemap tilemap; // Tilemap
    [SerializeField] GameObject tileCollision; // Tile Collision

    void Awake()
    {
        BoundsInt bounds = tilemap.cellBounds;

        Debug.Log(bounds.min);

        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                Vector3Int localPos = new(x, y, 0);

                if (tilemap.HasTile(localPos))
                {
                    GameObject obj = Instantiate(tileCollision, tilemap.transform);
                    obj.transform.localPosition = localPos;

                    ShadowTileCollider stc = obj.GetComponent<ShadowTileCollider>();
                    if (stc != null)
                    {
                        stc.tilemap = tilemap;
                        stc.tilePos = localPos;
                    }
                }
            }
        }
    }
}
