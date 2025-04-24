using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    [SerializeField] GameObject item;
    [SerializeField] Transform spawnPoint;
    [SerializeField] KeyGuide keyGuide;

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
        isInteracting = false;
        keyGuide.InActive();
    }

    void SpawnItem()
    {
        Instantiate(item, spawnPoint.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!isOpen) {
            isInteracting = true;
            keyGuide.Active();
        }
    }

    private void OnTriggerExit2D(Collider2D  other)
    {
        isInteracting = false;
        keyGuide.InActive();
    }
}
