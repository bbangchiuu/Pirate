using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using System.Linq;

public class VayneSkill : TuyetKy
{
    BehaviorTree[] behaviourList;

    private PlayerVisual playerVisual;
    PlayerMovement playerMovement;
    float timeAddEff = 0;

    private float AtkBuffDuration;
    public override void OnStart()
    {
        base.OnStart();
        Loai = LoaiTK.ChuDong;

        TKName = "VayneSkill";

        MaxCoolDown = ConfigManager.GetHeroSkillParams(TKName, 0);

        AtkBuffDuration = ConfigManager.GetHeroSkillParams(TKName, 4);

        var listMods = QuanlyNguoichoi.Instance.GetListRuntimeStatMods();
        if (listMods != null)
        {
            var mod = listMods.Find(e => e.Stat == EStatType.HEROPLUS && e.Target == "Vayne");
            if (mod != null)
            {
                AtkBuffDuration = ConfigManager.GetHeroSkillParams(TKName, 5);
            }
        }


        playerMovement = Unit.GetComponent<PlayerMovement>();
        behaviourList = Unit.gameObject.GetComponents<BehaviorTree>();

        playerVisual = Unit.visualGO.GetComponentInChildren<PlayerVisual>();

        Durability = 1000;
        //Durability -= QuanlyNguoichoi.Instance.SoLuotDungKyNang;
        //if (Durability < 0) Durability = 0;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (timeAddEff > 0)
        {
            timeAddEff -= Time.deltaTime;
            if (timeAddEff <= 0)
            {
                Unit.GetStatusEff().ApplyEffect(new BattleEffect(new EffectConfig
                {
                    Type = EffectType.BuffAtk,
                    Duration = AtkBuffDuration,
                    Param1 = ConfigManager.GetHeroSkillParams(TKName, 3),

                }));
            }
        }
    }

    public override bool Activate()
    {
        base.Activate();
        if (CoolDown <= 0 && Durability > 0)
        {
            CoolDown = MaxCoolDown;

            //float duration = ConfigManager.GetHeroSkillParams("GravesSkill", 1);
            //// effect have only purpose for visual
            ////Unit.GetStatusEff().ApplyEffect(new BattleEffect(new EffectConfig
            ////{
            ////    Type = EffectType.BatTu,
            ////    Duration = duration
            ////}));
            ////Unit.SetInvulnerableTime(duration);
            //Unit.SungPhu.IsActive = true;
            //Unit.GetComponent<PlayerMovement>().vuaDiVuaBan = true;
            //Unit.Animators[0].SetTrigger("start_skill");
            //playerVisual.SetVisualWeapon(false);
            //Unit.Animators[0].SetLayerWeight(Unit.Animators[0].GetLayerIndex("Upperbody"), 1f);
            //UpdateShooterStats();

            var be = behaviourList.First(e => e.Group == 1);
            if (be)
            {
                var dis = ConfigManager.GetHeroSkillParams(TKName, 1) / 100f;
                var time = ConfigManager.GetHeroSkillParams(TKName, 2) / 100f;
                var speed = dis / time;
                var dir = Unit.transform.forward * speed;
                be.SetVariableValue("TocDo", dir);
                be.SetVariableValue("ThoiGian", time);
                timeAddEff = time;

                be.EnableBehavior();
                be.Start();


                float rollspd = 0.5f / time;

                Unit.Animators[0].SetTrigger("skill");
                Unit.Animators[0].SetFloat("rollspd", rollspd);
            }

            return true;
        }

        return false;
    }
}
