using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpikePhase : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float cameraShakeIntensity;
    
    [SerializeField] private GameObject spikeFrame;
    [SerializeField] private GameObject areaWall;

    private bool isWork = false;

    private Rigidbody2D spikeFrameRigidBody;
    private Rigidbody2D areaWallRigidBody;

    void Start()
    {
        spikeFrameRigidBody = spikeFrame.GetComponent<Rigidbody2D>();
        areaWallRigidBody = areaWall.GetComponent<Rigidbody2D>();
    }

    public void SetWork(bool isWork)
    {
        if (this.isWork != isWork)
        {
            if (isWork)
            {
                CameraShake.instance.ShakeActiveCamera(cameraShakeIntensity);
            }
            else
            {
                CameraShake.instance.StopShakeActiveCamera();
            }
        }
        
        spikeFrameRigidBody.velocity = isWork ? Vector2.right * speed : Vector2.zero;
        areaWallRigidBody.velocity = isWork ? Vector2.right * speed : Vector2.zero;
        
        this.isWork = isWork;
    }
}
