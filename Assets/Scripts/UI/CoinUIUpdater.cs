using DG.Tweening;
using TMPro;
using UnityEngine;

public class CoinUIUpdater : MonoBehaviour
{
    private readonly string CoinTextTweeningID = "Coin";

    [SerializeField] TMP_Text coinText;

    int currentValue;


    private void Start()
    {
        UpdateCoinText(StageManager.Instance.gotCoin);
    }

    private void OnEnable()
    {
        StageManager.Instance.OnChangedCoin += UpdateCoin;
    }

    private void OnDisable()
    {
        StageManager.Instance.OnChangedCoin -= UpdateCoin;
    }

    public void UpdateCoin(int newCoin) {
        DOTween.Kill(CoinTextTweeningID);
        var tw = DOTween.To(() => currentValue, newValue => { currentValue = newValue; UpdateCoinText(currentValue); }, newCoin, 0.25f)
            .SetId(CoinTextTweeningID);
    }

    private void UpdateCoinText(int newCoin) {
        coinText.text = $"{newCoin}";
    }
}
