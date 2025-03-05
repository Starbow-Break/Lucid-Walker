using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Phase3BattleStartTrigger : MonoBehaviour
{
    [SerializeField] private List<string> targetTags;
    public UnityEvent triggerEvent;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(targetTags.Contains(collision.tag)) {
            triggerEvent.Invoke();
        }
    }
}
