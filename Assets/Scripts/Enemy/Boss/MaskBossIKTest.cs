using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class MaskBossIKTest : MonoBehaviour
{
    [Header("Bones")]
    [SerializeField] Transform body;
    [SerializeField] Transform frontHand;
    [SerializeField] Transform backHand;


    // Update is called once per frame
    void Start()
    {
        StartCoroutine(Move());
    }

    void Update()
    {
        frontHand.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.down);
        backHand.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.down);
    }

    IEnumerator Move() {
        while(true) {
            yield return Up();
            yield return Down();
        }
    }

    IEnumerator Up() {
        while(body.localPosition.y < 2f) {
            yield return null;
            body.localPosition += 2f * Time.deltaTime * Vector3.up;
        }
    }

    IEnumerator Down() {
        while(body.localPosition.y > -2f) {
            yield return null;
            body.localPosition += 2f * Time.deltaTime * Vector3.down;
        }
    }
}
