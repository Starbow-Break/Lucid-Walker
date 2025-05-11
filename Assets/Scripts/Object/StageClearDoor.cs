using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class StageClearDoor : MonoBehaviour
{
    [SerializeField] Key key; // 열쇠
    [SerializeField] Transform keyHole; // 열쇠 구멍
    [SerializeField] KeyGuide keyGuide;
    [SerializeField] private int stageNumber;

    [Header("Player")]
    [SerializeField] Animator playerAnim;
    [SerializeField] SpriteRenderer playerRenderer;

    Animator anim;
    GameObject interactingPlayer = null;
    bool isOpen = false;

    Vector2 openUiSize;
    Vector2 enterUiSize;

    #region 스테이지 클리어 UI
    [SerializeField] GameObject stageClearUI; // 스테이지 클리어 UI
    #endregion

    void Awake()
    {
        openUiSize = new Vector2(1.66f, 0.7f);
        enterUiSize = new Vector2(2.5f, 0.7f);
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isOpen && anim.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        {
            isOpen = true;
        }

        if (interactingPlayer != null && Input.GetKeyDown(KeyCode.Z))
        {
            if (isOpen)
            {
                StartCoroutine(StageClearSequence());
            }
            else
            {
                ItemFollowBag bag = interactingPlayer.GetComponent<ItemFollowBag>();
                if (bag != null && bag.HasItem(key))
                {
                    Open();
                }
            }
        }
    }

    void Open()
    {
        StartCoroutine(DoorOpenFlow());
    }

    IEnumerator DoorOpenFlow()
    {
        keyGuide.InActive();

        // ItemFollowBag에서 해당 열쇠 분리
        ItemFollowBag bag = interactingPlayer.GetComponent<ItemFollowBag>();

        // 맞는 열쇠가 있다면 문을 연다.
        if (bag.HasItem(key))
        {
            bag.RemoveItem(key);

            key.isFollow = true;
            key.FollowTarget(keyHole.position);
            while ((keyHole.position - key.transform.position).sqrMagnitude >= 0.001f)
            {
                yield return null;
            }

            // 문 열기
            Destroy(key.gameObject);
            anim.SetTrigger("open");
            yield return null;
        }

        if (interactingPlayer)
        {
            keyGuide.Active(KeyCode.Z, "들어가기", enterUiSize);
        }
    }

    IEnumerator StageClearSequence()
    {
        interactingPlayer = null;
        keyGuide.InActive();
        yield return PlayerEnterSequence();
        StartCoroutine(ActStageClearSequence());
    }

    IEnumerator PlayerEnterSequence()
    {
        Sequence sq = DOTween.Sequence().SetAutoKill(false);
        bool isComplete = false;

        float dist = (transform.position - playerAnim.transform.position).magnitude;
        float duration = dist / 3.0f;

        sq.OnStart(() => playerAnim.Play("MoveDoor"));
        sq.Append(playerAnim.transform.DOMoveX(transform.position.x, duration)
            .SetEase(Ease.Linear));
        sq.AppendCallback(() => {
            playerAnim.Play("Idle");
        });

        sq.AppendInterval(0.5f);

        sq.AppendCallback(() => {
            playerAnim.Play("TurnBack");
        });
        sq.AppendInterval(1.5f);
        sq.AppendCallback(() => {
            playerAnim.Play("BackWalk");
        });
        sq.Append(playerRenderer.DOColor(new Color(1f, 1f, 1f, 0f), 2f));
        sq.AppendInterval(0.5f);
        
        sq.OnComplete(() => isComplete = true);

        yield return new WaitUntil(() => isComplete);
    }

    IEnumerator ActStageClearSequence()
    {
        // UI 활성화
        stageClearUI.SetActive(true);

        var data = DataPersistenceManager.instance.GetCurrentGameData();

        // 에피소드 데이터 저장
        int currentEpisode = 1;
        foreach (var episode in data.episodesData)
        {
            if (episode.GetStageProgress(stageNumber) != null)
            {
                currentEpisode = episode.episodeNumber;
                break;
            }
        }

        data.lastPlayedEpisode = currentEpisode;

        var ep = data.GetEpisodeData(currentEpisode);
        var sp = ep.GetStageProgress(stageNumber);
        sp.isCleared = true;
        sp.gotTreasure = StageManager.Instance.gotTreasure;

        // currentStage 업데이트도 가능
        if (ep.currentStage < stageNumber)
            ep.currentStage = stageNumber + 1;

        data.returnFromStage = true;
        Debug.Log($"✅ 저장 전 returnFromStage = {data.returnFromStage}");

        data.gold += StageManager.Instance.gotCoin;

        DataPersistenceManager.instance.SaveGame();
        PlayerPrefs.Save();
        yield return null;

        // 2초 대기 (클리어 메시지 표시 시간)
        yield return new WaitForSeconds(5f);

        // 씬 전환
        LevelManager.Instance.LoadScene("Main", "CircleWipe");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isOpen)
            {
                interactingPlayer = other.gameObject;
                ItemFollowBag bag = other.GetComponent<ItemFollowBag>();
                if (bag != null && bag.HasItem(key))
                {
                    keyGuide.Active(KeyCode.Z, "열기", openUiSize);
                }
            }
            else
            {
                interactingPlayer = other.gameObject;
                keyGuide.Active(KeyCode.Z, "들어가기", enterUiSize);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (interactingPlayer == other.gameObject)
        {
            interactingPlayer = null;
            keyGuide.InActive();
        }
    }
}
