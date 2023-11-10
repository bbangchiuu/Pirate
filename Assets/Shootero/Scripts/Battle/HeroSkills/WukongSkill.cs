using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class WukongSkill : TuyetKy
{
    float timeActivate;

    PlayerMovement playerMovement;

    GameObject prefabSkill;

    public override void OnStart()
    {
        base.OnStart();
        Loai = LoaiTK.BiDong;
        TKName = "WukongSkill";
        timeActivate = ConfigManager.GetHeroSkillParams(TKName, 0);

        prefabSkill = Unit.transform.Find("WukongSkill").gameObject;
        playerMovement = Unit.GetComponent<PlayerMovement>();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (timeActivate > 0)
        {
            timeActivate -= Time.deltaTime;
        }
        else
        {
            if (playerMovement &&
                playerMovement.LastDirection.sqrMagnitude <= Vector3.kEpsilonNormalSqrt
                )
            {
                if (Unit.IsAlive() &&
                    Unit.LastTarget != null &&
                    Unit.LastTarget.gameObject &&
                    Unit.LastTarget.gameObject.activeInHierarchy)
                {
                    ActiveSkill(Unit.LastTarget);
                }
            }
        }
    }

    public void ActiveSkill(Transform target)
    {
        var wukongObj = ObjectPoolManager.Spawn(prefabSkill, Unit.transform.position, Unit.transform.rotation);

        if (target != null)
        {
            var dif = target.position - Unit.transform.position;
            dif.y = 0;
            if (dif.sqrMagnitude > Vector3.kEpsilonNormalSqrt)
            {
                wukongObj.transform.forward = dif.normalized;
            }
        }
        wukongObj.transform.localScale = Vector3.one;

        var st = wukongObj.GetComponentInChildren<GayThietBang>();
        var dmgScale = ConfigManager.GetHeroSkillParams(TKName, 1);
        var listMods = QuanlyNguoichoi.Instance.GetListRuntimeStatMods();
        if (listMods != null)
        {
            var mod = listMods.Find(e => e.Stat == EStatType.HEROPLUS && e.Target == "Wukong");
            if (mod != null && mod.Val > 0)
            {
                dmgScale = (int)Mathf.Round((float)mod.Val);

                var scaleObj = ConfigManager.GetWukongIllusionScale();
                if (scaleObj > 0)
                {
                    wukongObj.transform.localScale = Vector3.one * (scaleObj / 100f);
                }
            }
        }
        st.ActiveDan(0, Unit.GetCurStat().DMG * dmgScale / 100, Vector3.zero);
        var rot = st.root.GetComponentInChildren<TweenRotation>();
        rot.ResetToBeginning();
        rot.PlayForward();
        timeActivate = ConfigManager.GetHeroSkillParams(TKName, 0);
    }

    public override void OnUnitFired(Transform target)
    {
        base.OnUnitFired(target);
    }
}
