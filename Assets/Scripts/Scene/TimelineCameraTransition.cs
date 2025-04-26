using UnityEngine;
using Cinemachine;

public class TimelineCameraTransition : MonoBehaviour
{
    public CinemachineVirtualCamera activeCamera;
    public PolygonCollider2D newBoundingShape;
    public CircleWipe circleWipeEffect;
    public Transform newPositionTarget; // 이동시킬 대상 (예: dummyPlayer)

    private void Start()
    {
        if (circleWipeEffect == null)
        {
            circleWipeEffect = LevelManager.Circle;
            if (circleWipeEffect == null)
            {
                Debug.LogError("CircleWipeEffect를 찾을 수 없습니다!");
            }
        }

    }
    public void PlayTransition()
    {
        Debug.Log("📸 Timeline transition triggered!");

        if (circleWipeEffect != null)
        {
            circleWipeEffect.PlayTransition(false, true); // vertical=false, isEntering=true
        }

        if (activeCamera != null)
        {
            var confiner = activeCamera.GetComponent<CinemachineConfiner2D>();
            confiner.m_BoundingShape2D = newBoundingShape;
            confiner.InvalidateCache();
        }

        if (newPositionTarget != null)
        {
            newPositionTarget.position = transform.position;
        }
    }
}
