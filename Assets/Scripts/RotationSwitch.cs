using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSwitch : MonoBehaviour
{
  [SerializeField] GameObject gridObject;
  bool isInteracting = false;
  bool isRotate = false;

  [SerializeField] GameObject player;  // 플레이어 참조 추가

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
      gridObject.transform.rotation = Quaternion.Euler(0, 0, -90); // 절대 회전
      player.transform.rotation = Quaternion.Euler(0, 0, 0);      // 플레이어 회전
      isRotate = true;
    }
    else
    {
      gridObject.transform.rotation = Quaternion.Euler(0, 0, 0);  // 원래 상태로 복구
      player.transform.rotation = Quaternion.Euler(0, 0, 0);     // 플레이어 원래 상태로 복구
      isRotate = false;  // 다시 초기 상태로 돌아갈 수 있게 플래그 초기화
    }
  }
}
