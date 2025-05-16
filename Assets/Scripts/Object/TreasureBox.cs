using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    [System.Serializable]
    private struct SpawnData {
        public GameObject item;
        public int count;
    }

    [SerializeField] List<SpawnData> spawnDatas;
    [SerializeField] Transform spawnPoint;
    [SerializeField, Min(0f)] float spawnBound = 0.5f;
    [SerializeField] KeyGuide keyGuide;

    Animator anim;
    bool isInteracting = false;
    bool isOpen = false;

    void Awake() {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        bool gotTreasure = StageManager.Instance.gotTreasure;
        if (gotTreasure)
        {
            isOpen = true;
            anim.SetTrigger("already_open");
        }
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
        isInteracting = false;
        keyGuide.InActive();
        OpenSequance();
        
    }

    void OpenSequance()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOShakeScale(1f, strength: new Vector3(0.05f, 0.05f), vibrato: 10));
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() =>
        {
           anim.SetTrigger("open"); // 스프라이트 애니메이션
        });
        seq.Append(transform.DOPunchScale(new Vector3(-0.2f, 0.2f), 0.5f, vibrato: 3));
    }

    void SpawnItem()
    {
        foreach(var spawnData in spawnDatas)
        {
            for(int i = 0; i < spawnData.count; i++)
            {
                float offset = Random.Range(-spawnBound, spawnBound);
                GameObject spawnedItem = Instantiate(spawnData.item, spawnPoint.position, Quaternion.identity);
                //spawnedItem.transform.DOLocalJump(spawnPoint.position + offset * Vector3.right, 3.0f, 1, 1.0f, false);
                spawnedItem.transform.DOMove(spawnPoint.position + Vector3.up * 2f, 0.5f, false).SetEase(Ease.OutExpo);
            }
        }
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
