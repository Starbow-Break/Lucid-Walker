using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dress : MonoBehaviour, ICollectable
{
    public void Collect(GameObject owner)
    {
        StageManager.Instance.ActGetTreasure();
    }
}
