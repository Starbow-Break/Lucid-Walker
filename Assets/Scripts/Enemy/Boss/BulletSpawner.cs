using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] SmallMaskBullet bullet;
    [SerializeField] BulletSpawnerTargetArea targetArea;



    public void Spawn(int spawnCount = 10)
    {
        for(int i = 0; i < spawnCount; i++) {
            Vector2 targetPosition; // 목표 위치
            float dist; // 거리
            float dir;  // 방향

            targetPosition = targetArea.GetRandomPosition();

            float diff = targetPosition.x - transform.position.x;
            dist = Mathf.Abs(diff);
            dir = diff == 0 ? 0 : diff / dist;

            float y = transform.position.y - targetPosition.y;
            float theta = Random.Range(30f, 60f) * Mathf.Deg2Rad;

            SmallMaskBullet spawnedBullet = Instantiate(bullet, transform.position, Quaternion.identity);
            var bulletRb = spawnedBullet.GetComponent<Rigidbody2D>();
            float g = bulletRb.gravityScale * Physics.gravity.y;

            float sqrT = 2 * (y + dist * Mathf.Tan(theta)) / -g;
            if(sqrT < 0) {
                continue;
            }
            float t = Mathf.Sqrt(sqrT);
            float velocity = dist / (t * Mathf.Cos(theta));

            Vector2 shotVelocity = new(dir * velocity * Mathf.Cos(theta), Mathf.Abs(velocity) * Mathf.Sin(theta)); 
            spawnedBullet.SetVelocity(shotVelocity);
        }
    }
}