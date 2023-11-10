using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NgonLua : SatThuongDT
{
    public float DOT_Tick = 0.5f;
    public float Duration = 1f;

    TriggerEffect mTriggerEffect;
    BoxCollider boxCollider;
    BlazeLine lineVisual;
    private void Awake()
    {
        mTriggerEffect = gameObject.AddMissingComponent<TriggerEffect>();
        if (mTriggerEffect.Effect == null)
        {
            mTriggerEffect.Effect = new EffectConfig();
        }

        lineVisual = GetComponent<BlazeLine>();
        boxCollider = GetComponent<BoxCollider>();
    }

    protected override void OnEnable()
    {
        mTriggerEffect.Effect.Type = EffectType.DoT;
        mTriggerEffect.Effect.Damage = Damage;
        mTriggerEffect.Effect.Param1 = DOT_Tick;
        mTriggerEffect.TargetMask = LayersMan.Team1HitLayerMask;
        mTriggerEffect.Duration = Duration;
        mTriggerEffect.SourceTeamID = 2;
    }

    private void Start()
    {
        var indicator = SourceUnit.gameObject.GetComponentInChildren<BlazeLine>(true);
        if (indicator)
        {
            lineVisual.SetLocalTarget(indicator.LocalTarget);
        }

        if (boxCollider)
        {
            var size = boxCollider.size;
            size.z = lineVisual.LocalTarget.z;
            boxCollider.size = size;
            var center = boxCollider.center;
            center.z = lineVisual.LocalTarget.z * 0.5f;
            boxCollider.center = center;
        }
    }

    protected override bool OnCollisionOther(GameObject other)
    {
        return false;
    }
}
