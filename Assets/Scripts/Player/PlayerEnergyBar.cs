using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergyBar : MonoBehaviour
{
    public Transform character;
    public Vector3 offset;
    [SerializeField] private Slider energySlider;
    [SerializeField] private PlayerStats playerStats;

    private Vector3 lastPosition;

    void Update()
    {
        if (character != null && PlayerStats.Instance != null)
        {
            if (!PlayerStats.Instance.IsSinking)
            {
                // Sink 중이 아닐 때: 딱딱하게 정렬
                Vector3 targetPosition = character.position + offset;
                targetPosition.x = Mathf.Round(targetPosition.x * 100f) / 100f;
                targetPosition.y = Mathf.Round(targetPosition.y * 100f) / 100f;
                targetPosition.z = Mathf.Round(targetPosition.z * 100f) / 100f;

                if (targetPosition != lastPosition)
                {
                    transform.position = targetPosition;
                    lastPosition = targetPosition;
                }
            }
        }

        if (PlayerStats.Instance != null)
        {
            float currentEnergy = playerStats.CurrentEnergy;
            float maxEnergy = playerStats.MaxEnergy;
            energySlider.value = currentEnergy / maxEnergy;
        }
    }

    void LateUpdate()
    {
        if (character != null && playerStats != null)
        {
            if (playerStats.IsSinking)
            {
                Debug.Log($"PlayerEnergyBar sees sinking? {playerStats.IsSinking}");

                // Sink 중일 때는 부드럽게 따라감
                Vector3 targetPosition = character.position + offset;
                transform.position = Vector3.Lerp(transform.position, targetPosition, 20f * Time.deltaTime);
            }
        }
    }
}
