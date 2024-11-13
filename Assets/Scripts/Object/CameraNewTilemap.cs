using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
public class CameraNewTilemap : MonoBehaviour
{
    public CameraFollow cameraFollow;
    public Tilemap newTilemap;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Update the camera's target and tilemap
            cameraFollow.SetTarget(newTilemap);
        }
    }
}