using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class StageManager : MonoBehaviour
{
    [field: SerializeField] public int episode { get; private set; } = 1;
    [field: SerializeField] public int stage { get; private set; }  = 1;
    [SerializeField] HealthUI _healthUI;

    public static StageManager Instance { get; private set; }
    public bool gotTreasure { get; private set; }
    public int gotCoin {get; private set; }

    public Action<bool> OnChangedTreasure;
    public Action<int> OnChangedCoin;

    protected virtual void Awake()
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

    protected virtual void Start()
    {
        GameData gameData = DataPersistenceManager.instance.GetCurrentGameData();
        var episodeData = gameData?.GetEpisodeData(episode);
        var stageProgress = episodeData?.GetStageProgress(stage);

        gotTreasure = stageProgress != null ? stageProgress.gotTreasure : false;
        gotCoin = 0;
    }

    public void ActGetTreasure()
    {
        gotTreasure = true;
        OnChangedTreasure?.Invoke(gotTreasure);
    }

    public void AddCoin(int coin) {
        gotCoin += coin;
        OnChangedCoin?.Invoke(gotCoin);
    }
}
