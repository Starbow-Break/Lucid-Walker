using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] SmallMaskBullet bullet;
    [SerializeField] Vector2 spawnAreaMin;
    [SerializeField] Vector2 spawnAreaMax;

    public void Spawn(int spawnCount = 10)
    {
        for(int i = 0; i < spawnCount; i++) {
            Vector2 targetPosition; // 목표 위치
            float dist; // 거리
            float dir;  // 방향

            targetPosition = new(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            float diff = targetPosition.x - transform.position.x;
            dist = Mathf.Abs(diff);
            dir = diff == 0 ? 0 : diff / dist;

            float y = transform.position.y - targetPosition.y;
            float theta = Random.Range(30f, 60f) * Mathf.Deg2Rad;

            float sqrT = 2 * (y + dist * Mathf.Tan(theta)) / -Physics.gravity.y;
            if(sqrT < 0) {
                continue;
            }

            float t = Mathf.Sqrt(sqrT);
            float velocity = dist / (t * Mathf.Cos(theta));

            Debug.Log($"{sqrT} {t} {velocity}");

            Vector2 shotVelocity = new(dir * velocity * Mathf.Cos(theta), Mathf.Abs(velocity) * Mathf.Sin(theta)); 
            SmallMaskBullet spawnedBullet = Instantiate(bullet, transform.position, Quaternion.identity);
            spawnedBullet.SetVelocity(shotVelocity);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0, 0.5f);
        Vector2 areaCenter = (spawnAreaMin + spawnAreaMax) / 2;
        Vector2 areaSize = spawnAreaMax - spawnAreaMin;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}