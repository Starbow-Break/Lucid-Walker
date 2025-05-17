using UnityEngine;

public class Coin : MonoBehaviour, ICollectable
{
    [SerializeField] private int cost = 10;

    private SpriteRenderer _renderer;
    private Collider2D _collider;
    private AudioSource _sfx;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _sfx = GetComponent<AudioSource>();
    }

    // 획득
    public void Collect(GameObject owner)
    {
        GameData gameData = DataPersistenceManager.instance.GetCurrentGameData();
        int luck = gameData.luck;

        int coinDelta = CoinHelper.GetRaiseCoin(cost, luck);
        StageManager.Instance.AddCoin(coinDelta);

        _renderer.enabled = false;
        _collider.enabled = false;
        _sfx.Play();
        Destroy(gameObject, _sfx.clip.length);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어에게 닿으면 플레이어는 코인을 얻는다.
        if (other.CompareTag("Player"))
        {
            Collect(other.gameObject); // 코인 획득
        }
    }
}
