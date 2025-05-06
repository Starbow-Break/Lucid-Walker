using UnityEngine;

[DefaultExecutionOrder(-100)]
public class StageManager : MonoBehaviour
{
    [field: SerializeField] public int episode { get; private set; } = 1;
    [field: SerializeField] public int stage { get; private set; }  = 1;
    [SerializeField] HealthUI _healthUI;

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
        _healthUI.UpdateTreasureIcon(true);
    }
}
