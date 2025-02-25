using UnityEngine;
using UnityEngine.Events;

public class Phase3BattleStartTrigger : MonoBehaviour
{
    public UnityEvent triggerEvent;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")) {
            triggerEvent.Invoke();
        }
    }
}
