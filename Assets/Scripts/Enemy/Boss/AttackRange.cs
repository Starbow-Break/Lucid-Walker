using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    Transform rangeCoverTransform;

    void Start() {
        rangeCoverTransform = transform.GetChild(0);
    }

    public void SetProgress(float value) {
        rangeCoverTransform.localScale = new(
            rangeCoverTransform.localScale.x,
            value,
            rangeCoverTransform.localScale.z
        );
    }
}
