using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class BossSpriteController : MonoBehaviour
{
    [SerializeField] List<SpriteResolver> _resolvers;
    [SerializeField] MaskBossStats _stats;

    private void OnEnable()
    {
        _stats.OnDamage += OnDamage;
    }

    private void OnDisable()
    {
        _stats.OnDamage += OnDamage;
    }

    private void Start()
    {
        SetSpriteResolversToPhase(1);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetSpriteResolversToPhase(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetSpriteResolversToPhase(2);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetSpriteResolversToPhase(3);
        }
    } 

    public void OnDamage(float percent)
    {
        return;

        if (percent <= 15f)
        {
            SetSpriteResolversToPhase(3);
        }
        else if (percent <= 50f)
        {
            SetSpriteResolversToPhase(2);
        }
        else
        {
            SetSpriteResolversToPhase(1);
        }
    }

    private void SetSpriteResolversToPhase(int phase)
    {
        if (phase > 3 || phase < 1) return;

        Debug.Log(phase);
        string label = $"Phase3_{phase}";
        foreach (var resolver in _resolvers)
        {
            resolver.SetCategoryAndLabel(resolver.GetCategory(), label);
            resolver.ResolveSpriteToSpriteRenderer();
        }
    }
}
