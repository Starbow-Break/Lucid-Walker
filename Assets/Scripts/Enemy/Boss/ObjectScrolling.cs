using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectScrolling : MonoBehaviour
{
    [SerializeField] List<GameObject> platforms;
    
    public float scrollSpeed { get; set; } = 0.0f;

    void Update() {
        foreach(GameObject platformObj in platforms) {
            platformObj.transform.Translate(scrollSpeed * Time.deltaTime * Vector3.right);

            SpriteRenderer spriteRenderer = platformObj.GetComponent<SpriteRenderer>();
            TilemapRenderer tilemapRenderer = platformObj.GetComponent<TilemapRenderer>();
            
            float width = 0.0f;
            if (spriteRenderer != null) {
                width = spriteRenderer.size.x;
            }
            else if(tilemapRenderer != null) {
                width = tilemapRenderer.bounds.size.x;
            }

            if(width > 0.0f) {
                if(platformObj.transform.position.x > width) {
                    platformObj.transform.Translate(2.0f * width * Vector3.left);
                }
                else if(platformObj.transform.position.x < -width) {
                    platformObj.transform.Translate(2.0f * width * Vector3.right);
                }
            }
        }
    }
}
