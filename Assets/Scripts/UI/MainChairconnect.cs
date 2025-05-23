using System.Collections;
using UnityEngine;

public class MainChairconnect : MonoBehaviour
{
    public GameObject panel; // 활성화할 UI 패널
    public GameObject connectUI; // 활성화할 연결 UI 오브젝트
    public GameObject chairUI; // 빛나는 의자 effect ui

    private Animator chairAnimator; // 의자 오브젝트의 Animator


    #region ONOFF Animators
    // Set all of these up in the inspector
    [Header("Animator")]
    [SerializeField] private Animator televisionAnimator;
    [SerializeField] private Animator televisionAnimator2;
    [SerializeField] private Animator smalltv1_Animator;
    [SerializeField] private Animator smalltv2_Animator;
    [SerializeField] private Animator smalltv3_Animator;
    [SerializeField] private Animator smalltv4_Animator;
    [SerializeField] private Animator smalltv5_Animator;
    #endregion

    private bool isInteracting = false; // 상호작용 상태 체크
    private bool fromstage = false;
    [SerializeField] private bool skipInteractionOnReturn = false;

    [SerializeField] private GameObject player;
    [SerializeField] private Transform chairPosition;
    [SerializeField] private GameObject ep1panel;
    [SerializeField] private StageSelectManager stageSelectMgr;


    private void Start()
    {
        // 의자 오브젝트의 Animator 가져오기
        chairAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        // Player 태그를 가진 오브젝트와 충돌 시 처리
        if (other.CompareTag("Player"))
        {
            StartCoroutine(WaitForKeyPress(other.gameObject));
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
                    // 상호작용 시작
                    isInteracting = true;
                    if (stageSelectMgr != null)
                        stageSelectMgr.enabled = true;

                    // 플레이어 비활성화
                    // player.SetActive(false);
                    var movement = player.GetComponent<PlayerController>();
                    var renderer = player.GetComponent<SpriteRenderer>();

                    var rb = movement.GetComponent<Rigidbody2D>();
                    rb.isKinematic = true;
                    rb.velocity = Vector3.zero;
                    rb.Sleep();

                    movement.enabled = false;
                    renderer.enabled = false;

                    chairUI.SetActive(false);

                    // 의자 애니메이션 실행
                    if (chairAnimator != null)
                    {
                        chairAnimator.SetBool("Connect", true);
                    }

                    // UI 패널 활성화
                    if (panel != null)
                    {
                        panel.SetActive(true);
                    }

                    // 애니메이터 활성화
                    televisionAnimator.SetBool("On", true);
                    televisionAnimator2.SetBool("On", true);
                    smalltv1_Animator.SetBool("On", true);
                    smalltv2_Animator.SetBool("On", true);
                    smalltv3_Animator.SetBool("On", true);
                    smalltv4_Animator.SetBool("On", true);
                    smalltv5_Animator.SetBool("On", true);

                    // 연결 UI 활성화 후 2초 뒤 비활성화
                    if (connectUI != null)
                    {
                        connectUI.SetActive(true);
                        StartCoroutine(DisableConnectUI());
                    }
                }
                else
                {
                    // 상호작용 종료
                    isInteracting = false;

                    // 플레이어 다시 활성화
                    player.SetActive(true);
                    var movement = player.GetComponent<PlayerController>();
                    var renderer = player.GetComponent<SpriteRenderer>();

                    var rb = movement.GetComponent<Rigidbody2D>();
                    rb.isKinematic = false;
                    rb.velocity = Vector3.zero;

                    movement.enabled = true;
                    renderer.enabled = true;
                    chairUI.SetActive(true);


                    var data = DataPersistenceManager.instance.GetCurrentGameData();
                    if (fromstage)
                    {
                        Debug.Log("리턴");
                        // WaitForKeyPress() 의 상호작용 종료 블록
                        player.transform.localScale = new Vector3(0.72f, player.transform.localScale.y, 0.72f); // 항상 오른쪽 보기
                        data.returnFromStage = false;
                        DataPersistenceManager.instance.SaveGame();
                        fromstage = false;
                    }

                    // 의자 애니메이션 종료
                    if (chairAnimator != null)
                    {
                        chairAnimator.SetBool("Connect", false);
                    }

                    // 애니메이터 비활성화
                    televisionAnimator.SetBool("On", false);
                    televisionAnimator2.SetBool("On", false);
                    smalltv1_Animator.SetBool("On", false);
                    smalltv2_Animator.SetBool("On", false);
                    smalltv3_Animator.SetBool("On", false);
                    smalltv4_Animator.SetBool("On", false);
                    smalltv5_Animator.SetBool("On", false);

                    if (stageSelectMgr != null)
                        stageSelectMgr.enabled = false;

                    // UI 패널 비활성화
                    if (panel != null)
                    {
                        panel.SetActive(false);
                    }

                    // 연결 UI 비활성화
                    if (connectUI != null)
                    {
                        connectUI.SetActive(false);
                    }
                }
            }

            yield return null; // 매 프레임 대기
        }
    }
    public void StartAutoSequence()
    {
        StartCoroutine(AutoSequenceCoroutine());
    }
    private IEnumerator AutoSequenceCoroutine()
    {
        isInteracting = true;
        fromstage = true;

        // if (player != null)
        // {
        //     player.transform.position = chairPosition.position;
        //     player.SetActive(false);
        //     Debug.Log($"▶ StartAutoSequence() 호출됨, player active: {player.activeSelf}");
        // }

        if (chairUI != null)
            chairUI.SetActive(false);

        yield return null; // ✅ 한 프레임 기다려 Animator 초기화 시간 확보

        if (chairAnimator != null)
            chairAnimator.SetBool("Connect", true);

        if (panel != null)
            panel.SetActive(true);

        televisionAnimator.SetBool("On", true);
        televisionAnimator2.SetBool("On", true);
        smalltv1_Animator.SetBool("On", true);
        smalltv2_Animator.SetBool("On", true);
        smalltv3_Animator.SetBool("On", true);
        smalltv4_Animator.SetBool("On", true);
        smalltv5_Animator.SetBool("On", true);

        if (connectUI != null)
        {
            connectUI.SetActive(true);
            StartCoroutine(DisableConnectUI());
        }
        ep1panel.SetActive(true);
        StartCoroutine(WaitForKeyPress(player));

    }


    private IEnumerator DisableConnectUI()
    {
        yield return new WaitForSeconds(2f); // 2초 대기

        if (connectUI != null)
        {
            connectUI.SetActive(false); // 연결 UI 비활성화
        }
    }
}
