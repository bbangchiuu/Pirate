using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BayLuaVan : SatThuongDT
{
    public float DOT_Tick = 0.5f;
    public float Duration = 1f;

    TriggerEffect mTriggerEffect;
    private void Awake()
    {
        mTriggerEffect = gameObject.AddMissingComponent<TriggerEffect>();
        if (mTriggerEffect.Effect == null)
        {
            mTriggerEffect.Effect = new EffectConfig();
        }
        mTriggerEffect.Effect.Type = EffectType.DoT;
        mTriggerEffect.Effect.Damage = Damage;
        mTriggerEffect.Effect.Param1 = DOT_Tick;
        mTriggerEffect.TargetMask = LayersMan.Team1DefaultHitLayerMask;
        mTriggerEffect.Duration = Duration;
        mTriggerEffect.SourceTeamID = 2;
    }

    protected override bool OnCollisionOther(GameObject other)
    {
        //var proj = other.GetComponentInParent<BattleUnit>();
        //if (proj != null)
        //{
        //    proj.TakeDamage(Damage);
        //    mIsActive = false;
        //    return true;
        //}
        //else
        //{
        //    if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") ||
        //        other.gameObject.layer == LayerMask.NameToLayer("Default"))
        //    {
        //        mIsActive = false;
        //        return true;
        //    }
        //}

        return false;
    }

    //public override void ActiveDan(float speed, float dmg, Vector3 target)
    //{
    //    mIsActive = true;
    //    this.Damage = dmg;
    //}
}
