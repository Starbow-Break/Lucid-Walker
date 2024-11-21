using UnityEngine;

public class PortalCurtain : MonoBehaviour
{
    [SerializeField] private GameObject objectToDestroy; // 삭제할 오브젝트
    [SerializeField] private GameObject objectToEnableCollider; // BoxCollider2D를 활성화할 오브젝트

    // 특정 로직 실행 함수
    public void ActivateCurtain()
    {
        // 지정된 오브젝트 삭제
        if (objectToDestroy != null)
        {
            Destroy(objectToDestroy); // 오브젝트 삭제
        }

        // 다른 오브젝트의 BoxCollider2D 활성화
        if (objectToEnableCollider != null)
        {
            BoxCollider2D collider = objectToEnableCollider.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.enabled = true; // Collider 활성화
            }
        }
    }
}
