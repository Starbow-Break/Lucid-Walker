using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

enum HiddenWallTriggerType {
    Enter, Exit
}

public class HiddenWallTrigger : MonoBehaviour
{
    [SerializeField] HiddenRoomWall hiddenRoomWall;
    [SerializeField] HiddenWallTriggerType type;

    private void OnTriggerEnter2D(Collider2D other) {
        switch(type) {
            case HiddenWallTriggerType.Enter : {
                hiddenRoomWall.Enter();
                break;
            }
            case HiddenWallTriggerType.Exit : {
                hiddenRoomWall.Exit();
                break;
            }
            default: break;
        }
    }
}
