using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSkill : Skill
{   
    [SerializeField] GameObject yellowLamp;
    [SerializeField] GameObject redLamp;
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] Animator anim;

    protected override IEnumerator SkillFlow()
    {
        anim.SetTrigger("skill_light");

        List<MaskBossLamp> lamps = new();
        List<IEnumerator> forwardCoroutines = new();
        List<IEnumerator> backwardCoroutines = new();

        foreach(Transform tf in spawnPoints) {
            float randomValue = Random.Range(0.0f, 100.0f);
            GameObject lampObj = Instantiate((randomValue >= 50.0f ? redLamp : yellowLamp), tf.position, Quaternion.identity);
            MaskBossLamp lamp = lampObj.GetComponent<MaskBossLamp>();
            lamps.Add(lamp);
            forwardCoroutines.Add(lamp.MoveLampFlow(tf.position, tf.position - 3.5f * Vector3.up, 1.0f));
            backwardCoroutines.Add(lamp.MoveLampFlow(tf.position - 3.5f * Vector3.up, tf.position, 1.0f));
        }

        yield return new WaitForCoroutines(this, forwardCoroutines);

        SetAllOfLamps(lamps, true);
        yield return new WaitForSeconds(2.0f);

        SetAllOfLamps(lamps, false);
        yield return new WaitForCoroutines(this, backwardCoroutines);
        foreach(MaskBossLamp lamp in lamps) {
            Destroy(lamp);
        }
    }

    void SetAllOfLamps(List<MaskBossLamp> lamps, bool isOn) {
        foreach(MaskBossLamp lamp in lamps) {
            lamp.SetLamp(isOn);
        }
    }
}
