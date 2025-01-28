using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableBossStageWalkingMonster : WalkingMonster
{
    [Header("Shoot")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform shootPoint;
    
    public override void PerformAttack() {
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        float dir = transform.localScale.x > 0 ? 1 : -1;
        bullet.transform.localScale = new Vector3(dir, 1, 1);
    }
}
