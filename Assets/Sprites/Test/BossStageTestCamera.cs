using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStageTestCamera : MonoBehaviour
{
    int phase = 1;

    private void Update() {
        if(phase < 3 && Input.GetKeyDown(KeyCode.W))  {
            ++phase;
            transform.position -= 20.0f * Vector3.up;
        }

        if(phase > 1 && Input.GetKeyDown(KeyCode.Q))  {
            --phase;
            transform.position += 20.0f * Vector3.up;
        }   
    }
}
