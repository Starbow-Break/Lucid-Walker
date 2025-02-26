using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScrolling : MonoBehaviour
{
    [SerializeField] List<GameObject> platforms;
    
    public float scrollSpeed { get; set; } = 0.0f;

    void Update() {
        foreach(GameObject platformObj in platforms) {
            platformObj.transform.Translate(scrollSpeed * Time.deltaTime * Vector3.right);
            
            if(platformObj.transform.position.x > 34.0f) {
                platformObj.transform.Translate(68.0f * Vector3.left);
            }
            else if(platformObj.transform.position.x < -34.0f) {
                platformObj.transform.Translate(68.0f * Vector3.right);
            }
        }
    }
}
