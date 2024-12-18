using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trolley : MonoBehaviour
{
    [SerializeField] Zipline zipline;
    PlayerController interactingPlayerController = null; // 상호작용중인 플레이어

    // Update is called once per frame
    void Update()
    {
        if(zipline && Input.GetKeyDown(KeyCode.Z) && interactingPlayerController) {
            zipline.BoardPlayer(interactingPlayerController);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            interactingPlayerController = other.GetComponent<PlayerController>();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            interactingPlayerController = null;
        }
    }
}
