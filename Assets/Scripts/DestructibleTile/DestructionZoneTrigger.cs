using UnityEngine;
using System.Collections.Generic;

public class DestructionZoneTrigger : MonoBehaviour
{
    public List<DestructionSourceHandler> destructionHandlers = new List<DestructionSourceHandler>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
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
