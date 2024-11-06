using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

enum HiddenWallTriggerType {
    Enter, Exit
}

public class HiddenWallTrigger : MonoBehaviour
{
    [SerializeField] HiddenWall hiddenWall;
    [SerializeField] HiddenWallTriggerType type;

    private void OnTriggerEnter2D(Collider2D other) {
        switch(type) {
            case HiddenWallTriggerType.Enter : {
                hiddenWall.Enter();
                break;
            }
            case HiddenWallTriggerType.Exit : {
                hiddenWall.Exit();
                break;
            }
            default: break;
        }
    }
}
