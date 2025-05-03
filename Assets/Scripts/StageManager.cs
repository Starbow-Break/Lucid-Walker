using UnityEngine;

[DefaultExecutionOrder(-100)]
public class StageManager : MonoBehaviour
{
    [SerializeField] private int episode = 1;
    [SerializeField] private int stage = 1;

    public static StageManager Instance { get; private set; }
    public bool gotTreasure { get; private set; }

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else {
            if(Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    protected void Start()
    {
        GameData gameData = DataPersistenceManager.instance.GetCurrentGameData();
        var episodeData = gameData.GetEpisodeData(episode);
        var stageProgress = episodeData.GetStageProgress(stage);

        gotTreasure = stageProgress.gotTreasure;
    }

    public void ActGetTreasure()
    {
        gotTreasure = true;
    }
}
