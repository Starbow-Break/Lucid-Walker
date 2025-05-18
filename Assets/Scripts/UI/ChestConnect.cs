using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestConnect : MonoBehaviour
{
    public GameObject uiPanel;

    private bool isPlayerInside = false;

    private void Update()
    {
        if (!isPlayerInside) return;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            bool isActive = uiPanel.activeSelf;
            uiPanel.SetActive(!isActive);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = false;
            uiPanel.SetActive(false); // 나가면 무조건 닫기
        }
    }

    public void OnCloseButtonPressed()
    {
        uiPanel.SetActive(false);
    }
}

