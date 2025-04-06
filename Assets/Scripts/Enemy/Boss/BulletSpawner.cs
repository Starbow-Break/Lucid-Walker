using DG.Tweening;
using UnityEditor.Callbacks;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] SmallMaskBullet bullet;
    [SerializeField] Vector2 spawnAreaMin;
    [SerializeField] Vector2 spawnAreaMax;

    public void Spawn(int spawnCount = 10)
    {
        for(int i = 0; i < spawnCount; i++) {
            Vector2 targetPosition;
            float dist;
            float direction = transform.right.x;

            targetPosition = new(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );
            dist = targetPosition.x - transform.position.x;

            float y = transform.position.y - targetPosition.y;
            float theta = Random.Range(30f, 60f) * Mathf.Deg2Rad;

            float sqrVelocity = Physics.gravity.y * dist * dist / (2 * Mathf.Pow(Mathf.Cos(theta), 2) * (-y - dist * Mathf.Tan(theta)));
            if(sqrVelocity < 0) continue;
            float velocity = Mathf.Sqrt(sqrVelocity) * dist / Mathf.Abs(dist);

            SmallMaskBullet spawnedBullet = Instantiate(bullet, transform.position, Quaternion.identity);
            spawnedBullet.SetVelocity(new(velocity * Mathf.Cos(theta), Mathf.Abs(velocity * Mathf.Sin(theta))));
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