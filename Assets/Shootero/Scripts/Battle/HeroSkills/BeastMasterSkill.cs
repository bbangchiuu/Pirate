using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastMasterSkill : TuyetKy
{
    DonViChienDau mChimObj;
    BehaviorDesigner.Runtime.BehaviorTree mBehavior;

    Queue<AtkCommand> queueTargets = new Queue<AtkCommand>();

    void InitUnit()
    {
        if (mChimObj == null)
        {
            mChimObj = GameObject.Instantiate(Resources.Load<DonViChienDau>("AIPrefabs/ChimBM"));
            var behaviours = mChimObj.GetComponents<BehaviorDesigner.Runtime.BehaviorTree>();
            foreach (var b in behaviours)
            {
                if (b.Group == 0) // find bird main behaviour
                {
                    mBehavior = b;
                    break;
                }
            }
        }

        TKName = "BeastMasterSkill";

        if (mBehavior)
        {
            mBehavior.SetVariableValue("AttackRate",
                ConfigManager.GetHeroSkillParams(TKName, 0) / 100f);
        }
        if (mChimObj)
        {
            mChimObj.SetInvulnerableTime(10000f);
        }
    }
    public override void OnStart()
    {
        base.OnStart();
        Loai = LoaiTK.BiDong;
        TKName = "BeastMasterSkill";
        InitUnit();
    }

    public void CleanUpObj()
    {
        if (mChimObj)
        {
            mChimObj.gameObject.SetActive(false);
            GameObject.Destroy(mChimObj.gameObject);
            mChimObj = null;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (QuanlyNguoichoi.Instance)
        {
            if (QuanlyNguoichoi.Instance.IsEndGame ||
                QuanlyNguoichoi.Instance.gameObject.activeSelf == false)
            {
                CleanUpObj();
            }
        }
        if (mChimObj)
        {
            // duongrs code chong liet chim
            if (mBehavior)
            {
                if (BehaviorDesigner.Runtime.BehaviorManager.instance.IsBehaviorEnabled(mBehavior) == false)
                {
                    mBehavior.EnableBehavior();
                }
            }
        }
    }

    public void TeleportBackToUnit()
    {
        if (mChimObj == null) InitUnit();
        if (mChimObj == null) return;

        mChimObj.Warp(Unit.transform.position);
        mChimObj.SetInvulnerableTime(10000);
    }

    public override void OnUnitFired(Transform target)
    {
        base.OnUnitFired(target);
        if (mBehavior)
        {
            if (QuanlyNguoichoi.Instance &&
                QuanlyNguoichoi.Instance.PlayerUnit &&
                QuanlyNguoichoi.Instance.HaveHeroPlus("BeastMaster"))
            {
                var curStat = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat();
                mChimObj.OverrideChiSoCrit(curStat.CRIT, curStat.CRIT_DMG);
            }

            var listMods = QuanlyNguoichoi.Instance.GetListRuntimeStatMods();
            if (listMods != null)
            {
                var mod = listMods.Find(e => e.Stat == EStatType.HEROPLUS && e.Target == "BeastMaster");
                if (mod != null)
                {
                    Durability = (int)Mathf.Round((float)mod.Val);
                }
            }

            var dmg = (int)Mathf.Round(Unit.GetCurStat().DMG *
                ConfigManager.GetHeroSkillParams(TKName, 1) / 100f);
            //var animator = mChimObj.GetComponent<Animator>();
            //animator.SetTrigger("atk");
            //var unit = target.GetComponent<DonViChienDau>();
            var atkRate = ConfigManager.GetHeroSkillParams(TKName, 0);
            if (Random.Range(0, 100) < atkRate)
            {
                queueTargets.Enqueue(new AtkCommand() { T = target, D = dmg });
            }

            //mChimObj.OverrideChiSoDam(dmg);
            //mBehavior.SetVariableValue("playerTarget", target);
            //mBehavior.SendEvent("KhiNguoiChoiBan");
        }
    }

    public override void OnUnitDied()
    {
        //if (mChimObj)
        //{
        //    mChimObj.TakeDamage(mChimObj.GetCurHP() + 10);
        //}
    }

    public AtkCommand GetAtkCommand()
    {
        if (queueTargets.Count == 0) return null;
        var peek = queueTargets.Peek();
        if (peek != null && peek.T != null)
        {
            return peek;
        }
        return null;
    }

    public AtkCommand Dequeue()
    {
        var cmd = queueTargets.Dequeue();
        return cmd;
    }
}

public class AtkCommand
{
    public Transform T;
    public long D;
}