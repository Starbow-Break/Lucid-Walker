using UnityEngine;

public class LeverController : MonoBehaviour
{
    [SerializeField] private WaterController waterController;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            if (Input.GetKeyDown(KeyCode.A))
            {

                // A 키를 눌렀을 때 물 높이 증가
                if (waterController != null)
                {
                    waterController.IncreaseWaterLevel();
                }
            }
        }
    }
}
