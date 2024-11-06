using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UIElements;

public class TreasureBox : MonoBehaviour
{
    [SerializeField] GameObject item;
    [SerializeField] Transform spawnPoint;

    Animator anim;
    bool isInteracting = false;
    bool isOpen = false;

    void Awake() {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(isInteracting && !isOpen && Input.GetKeyDown(KeyCode.Z)) {
            Open();
        }
    }

    void Open()
    {
        isOpen = true;
        anim.SetTrigger("open");
    }

    void SpawnItem()
    {
        Instantiate(item, spawnPoint.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        isInteracting = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        isInteracting = false;
    }
}
