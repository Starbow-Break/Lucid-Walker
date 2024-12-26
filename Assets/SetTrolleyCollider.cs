using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTrolleyCollider : MonoBehaviour
{
    GameObject interactingPlayer = null;
    Zipline zipline;

    void Awake() {
        zipline = transform.parent.GetComponent<Zipline>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if(interactingPlayer && Input.GetKeyDown(KeyCode.Z)) {
            Debug.Log("Yay");
            DoSetTrolley();
        }
    }

    void DoSetTrolley() {
        ItemFollowBag bag = interactingPlayer.GetComponent<ItemFollowBag>();
        Trolley trolley = zipline.GetTargetTrolley();

        if(bag != null && bag.HasItem(trolley)) {
            zipline.SetTrolley(bag, trolley);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactingPlayer = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (interactingPlayer == other.gameObject)
        {
            interactingPlayer = null;
        }
    }
}
