using System.Collections;
using UnityEngine;

public class RotationSwitch : MonoBehaviour
{
    [SerializeField] GameObject gridObject;
    bool isInteracting = false;
    bool isRotate = false;

    [SerializeField] GameObject player;  // 플레이어 참조 추가
    Animator anim;

    public float rotationDuration = 1f; // 회전이 걸리는 시간 (초)
    private Quaternion initialRotation;
    private Quaternion targetRotation;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isInteracting && Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Z key pressed");
            RotateGrid();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isInteracting && other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger");
            isInteracting = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (isInteracting && other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player exited the trigger");
            isInteracting = false;
        }
    }

    void RotateGrid()
    {
        if (!isRotate)
        {
            anim.SetTrigger("Rotate");

            // 플레이어 즉시 회전
            player.transform.rotation = Quaternion.Euler(0, player.transform.rotation.eulerAngles.y, 90);      // 플레이어 회전

            // 회전 목표 설정 (-90도)
            initialRotation = gridObject.transform.rotation;
            targetRotation = Quaternion.Euler(0, 0, -90);


            // 카메라 흔들림 시작
            FindObjectOfType<CameraFollow>().TriggerShake();

            // Coroutine을 통해 천천히 회전
            StartCoroutine(RotateOverTime(targetRotation));

            isRotate = true;
        }
        else
        {
            anim.SetTrigger("Rotate");

            // 플레이어 즉시 회전
            player.transform.rotation = Quaternion.Euler(0, player.transform.rotation.eulerAngles.y, 0);

            // 회전 목표 설정 (0도로 되돌리기)
            initialRotation = gridObject.transform.rotation;
            targetRotation = Quaternion.Euler(0, 0, 0);

            // 카메라 흔들림 시작
            FindObjectOfType<CameraFollow>().TriggerShake();

            // Coroutine을 통해 천천히 회전
            StartCoroutine(RotateOverTime(targetRotation));

            isRotate = false;
        }
    }

    IEnumerator RotateOverTime(Quaternion targetRotation)
    {
        float timeElapsed = 0f;

        while (timeElapsed < rotationDuration)
        {
            // Lerp를 사용하여 천천히 회전
            gridObject.transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, timeElapsed / rotationDuration);

            // 시간을 증가시켜서 다음 프레임에 더 많이 회전하게 함
            timeElapsed += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 마지막으로 정확하게 목표 각도에 맞추기
        gridObject.transform.rotation = targetRotation;
    }
}
