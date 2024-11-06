using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class ShadowTileCollider : MonoBehaviour
{
    public Tilemap tilemap;
    public Vector3Int tilePos;

    TileBase tile;
    public int lightCount;

    void Awake() { 
        lightCount = 0;
    }

    void Start()
    {
        tile = tilemap.GetTile(tilePos);
        tilemap.SetTile(tilePos, null);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Mask"))
        {
            if(lightCount == 0) {
                tilemap.SetTile(tilePos, tile);
            }

            Interlocked.Increment(ref lightCount);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Mask"))
        {
            Interlocked.Decrement(ref lightCount);

            if(lightCount == 0) {
                tilemap.SetTile(tilePos, null);
            }
        }
    }
}
