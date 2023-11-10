using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ANTICHEAT
using Int64 = CodeStage.AntiCheat.ObscuredTypes.ObscuredLong;
using Int32 = CodeStage.AntiCheat.ObscuredTypes.ObscuredInt;
using Float = CodeStage.AntiCheat.ObscuredTypes.ObscuredFloat;
using Bool = CodeStage.AntiCheat.ObscuredTypes.ObscuredBool;
#else
using Int64 = System.Int64;
using Int32 = System.Int32;
using Float = System.Single;
using Bool = System.Boolean;
#endif

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("StartSelfExplosion")]
    [TaskCategory("Shootero")]
    public class StartSelfExplosion : Action
    {
        public LayerMask layerTarget;
        public float delayTime = 0.5f;
        public float radius = 3f;
        public bool overrideDMG = false;
        public long DMG = 0;

        DonViChienDau unit;
        SelfExplosion selfExp;
        public override void OnAwake()
        {
            base.OnAwake();
            unit = gameObject.GetComponent<DonViChienDau>();

            if (layerTarget.value == LayersMan.Team1LegacyHitLayerMask)
            {
                layerTarget.value = LayersMan.Team1HitLayerMask;
            }
            else if (layerTarget.value == LayersMan.Team1OnlyHitLayerMask)
            {
                layerTarget.value = LayersMan.Team1DefaultHitLayerMask;
            }
        }
        public override void OnStart()
        {
            if (unit == null) return;

            selfExp = gameObject.GetComponent<SelfExplosion>();
            if (selfExp == null)
            {
                selfExp = gameObject.AddComponent<SelfExplosion>();
            }
            long dmg = overrideDMG ? DMG : (long)unit.GetStat().DMG;

            selfExp.Activate(radius, dmg, layerTarget, delayTime);
        }

        public override TaskStatus OnUpdate()
        {
            if (unit == null || selfExp == null) return TaskStatus.Failure;

            if (selfExp.IsExploded == false)
            {
                return TaskStatus.Running;
            }
            else
            {
                return TaskStatus.Success;
            }
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
        }
    }
}
