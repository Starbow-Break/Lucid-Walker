using UnityEngine;
using System.Collections.Generic;

public class DestructionZoneTrigger : MonoBehaviour
{
    public List<DestructionSourceHandler> destructionHandlers = new List<DestructionSourceHandler>();
    public List<string> targetTags;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach(string tag in targetTags) {
            if (collision.CompareTag(tag))
            {
                Debug.Log("[DestructionZoneTrigger] Enemy entered destruction zone.");

                // 리스트에 있는 모든 DestructionSourceHandler 실행
                foreach (var handler in destructionHandlers)
                {
                    if (handler != null)
                    {
                        handler.TriggerDestruction();
                    }
                }
            }
        }
    }
}
