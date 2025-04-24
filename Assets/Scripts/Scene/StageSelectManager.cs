using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectManager : MonoBehaviour, IDataPersistence
{
    public GameObject ep1_panel;
    public List<StageIcon> stageIcons; // STAGE1 ~ STAGE7 UI
    public Transform character; // 지금 선택된 캐릭터 오브젝트
    [SerializeField] private float moveDuration = 0.5f;

    private Coroutine moveCoroutine;

    private GameData gameData;
    private int currentStageIndex = 0;
    private Dictionary<int, Dictionary<string, int>> movementMap = new()
{
    { 1, new() { { "right", 2 } } },
    { 2, new() { { "left", 1 }, { "right", 3 } } },
    { 3, new() { { "left", 2 }, { "up", 4 } } },
    { 4, new() { { "down", 3 }, { "left", 5 } } },
    { 5, new() { { "right", 4 }, { "left", 6 } } },
    { 6, new() { { "right", 5 },{ "up", 7 } } },
    { 7, new() { { "down", 6 } } }
};





    private void Awake()
    {
        for (int i = 0; i < stageIcons.Count; i++)
        {
            stageIcons[i].Setup(i, this);
        }
    }

    public void LoadData(GameData data)
    {
        gameData = data;
        UpdateAllStageIcons();
        MoveCharacterToStage(currentStageIndex);
    }

    public void SaveData(ref GameData data)
    {
        data.GetEpisodeData(1).currentStage = currentStageIndex + 1;
    }

    private void Update()
    {
        if (ep1_panel == null || !ep1_panel.activeSelf) return;
        if (Input.GetKeyDown(KeyCode.RightArrow)) Move("right");
        if (Input.GetKeyDown(KeyCode.LeftArrow)) Move("left");
        if (Input.GetKeyDown(KeyCode.UpArrow)) Move("up");
        if (Input.GetKeyDown(KeyCode.DownArrow)) Move("down");
        if (Input.GetKeyDown(KeyCode.Return)) TryEnterStage();
    }

    private void Move(string dir)
    {
        int currentStage = currentStageIndex + 1; // 1-based 스테이지 번호
        if (!movementMap.ContainsKey(currentStage) || !movementMap[currentStage].ContainsKey(dir))
        {
            Debug.Log($"❌ Stage {currentStage}에서 {dir} 방향은 이동 불가");
            return;
        }

        int nextStage = movementMap[currentStage][dir];
        currentStageIndex = nextStage - 1; // 다시 0-based 인덱스로 환산
        MoveCharacterToStage(currentStageIndex);

        // UpdateAllStageIcons();
        // var ep = gameData.GetEpisodeData(1);
        // var sp = ep.GetStageProgress(next + 1);

        // // 이동하려는 다음 스테이지가 클리어되어 있거나 현재 인덱스보다 작으면 허용
        // if (sp.isCleared || next < currentStageIndex)
        // {
        //     currentStageIndex = next;
        //     MoveCharacterToStage(currentStageIndex);
        //     UpdateAllStageIcons();
        // }
        // else
        // {
        //     Debug.Log("❌ 아직 클리어하지 않은 스테이지입니다.");
        // }
    }

    private void TryEnterStage()
    {
        var ep = gameData.GetEpisodeData(1);
        var sp = ep.GetStageProgress(currentStageIndex + 1);
        // 🔥 테스트용: 클리어 여부 무시하고 카툰/스테이지 입장
        if (sp.hasCartoonScene && sp.cartoonSceneTriggerTime == CartoonSceneTriggerTime.BeforeStage && !sp.cartoonScenePlayed)
        {
            CartoonSceneManager.Instance.PlayCartoon(sp.stageNumber, () =>
            {
                sp.cartoonScenePlayed = true;
                EnterStage(sp.stageNumber);
            });
        }
        else
        {
            EnterStage(sp.stageNumber);
        }
        // if (sp.hasCartoonScene && sp.cartoonSceneTriggerTime == CartoonSceneTriggerTime.BeforeStage && !sp.cartoonScenePlayed)
        // {
        //     CartoonSceneManager.Instance.PlayCartoon(sp.stageNumber, () =>
        //     {
        //         sp.cartoonScenePlayed = true;
        //         EnterStage(sp.stageNumber);
        //     });
        // }
        // else
        // {
        //     EnterStage(sp.stageNumber);
        // }
    }

    private void EnterStage(int stageNumber)
    {
        LevelManager.Instance.LoadScene($"Stage{stageNumber}", "CircleWipe");
    }

    public void TryEnterStageByClick(int index)
    {
        currentStageIndex = index;
        MoveCharacterToStage(currentStageIndex);
        UpdateAllStageIcons();
        TryEnterStage();
    }
    private void MoveCharacterToStage(int index)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        Vector3 stagePos = stageIcons[index].transform.position;
        stagePos.y += 0.5f;
        moveCoroutine = StartCoroutine(SmoothMove(character, stagePos, moveDuration));
    }

    private IEnumerator SmoothMove(Transform target, Vector3 destination, float duration)
    {
        Vector3 start = target.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            target.position = Vector3.Lerp(start, destination, elapsed / duration);
            yield return null;
        }

        target.position = destination;

        UpdateAllStageIcons();

    }


    private void UpdateAllStageIcons()
    {
        var ep = gameData.GetEpisodeData(1);
        for (int i = 0; i < stageIcons.Count; i++)
        {
            var sp = ep.GetStageProgress(i + 1);
            stageIcons[i].UpdateUI(sp, i == currentStageIndex);
        }
    }

    // 클리어 후 호출용
    public void HandleStageClear(int stageNumber, bool gotTreasure)
    {
        var ep = gameData.GetEpisodeData(1);
        var sp = ep.GetStageProgress(stageNumber);
        sp.isCleared = true;
        sp.gotTreasure = gotTreasure;

        if (sp.hasCartoonScene && sp.cartoonSceneTriggerTime == CartoonSceneTriggerTime.AfterStage && !sp.cartoonScenePlayed)
        {
            CartoonSceneManager.Instance.PlayCartoon(stageNumber, () =>
            {
                sp.cartoonScenePlayed = true;
                MoveToNextStage(stageNumber);
            });
        }
        else
        {
            MoveToNextStage(stageNumber);
        }
    }

    private void MoveToNextStage(int stageNumber)
    {
        var nextIndex = Mathf.Clamp(stageNumber, 0, stageIcons.Count - 1);
        currentStageIndex = nextIndex;
        MoveCharacterToStage(currentStageIndex);
        UpdateAllStageIcons();
    }
}
