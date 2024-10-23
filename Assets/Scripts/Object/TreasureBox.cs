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

    void Awake() {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(isInteracting && Input.GetKeyDown(KeyCode.Z)) {
            Open();
        }
    }

    void Open()
    {
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
