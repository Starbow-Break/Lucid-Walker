using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestConnect : MonoBehaviour
{
    public GameObject uiPanel;

    private bool isInteracting = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(WaitForKeyPress(collision.gameObject));
        }

    }

    private IEnumerator WaitForKeyPress(GameObject player)
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (!isInteracting)
                {
                    isInteracting = true;
                    uiPanel.SetActive(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Z) && isInteracting)
            {
                isInteracting = false;
                uiPanel.SetActive(false);
            }

            yield return null;
        }
    }

    public void OnCloseButtonPressed()
    {
        if (!isInteracting) return;
        isInteracting = false;
        uiPanel.SetActive(false);
    }
}

