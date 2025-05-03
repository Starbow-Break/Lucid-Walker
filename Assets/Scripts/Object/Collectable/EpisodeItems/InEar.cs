using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InEar : MonoBehaviour, ICollectable
{
    public void Collect(GameObject owner)
    {
        StageManager.Instance.ActGetTreasure();
    }
}
