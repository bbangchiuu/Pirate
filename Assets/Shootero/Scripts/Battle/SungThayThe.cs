using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SungThayThe : MonoBehaviour
{
    public SungCls sung1;
    public SungCls sung2;
    public SungCls sung3;
    public SungCls sung4;
    public string animTrigger;
    public bool IsActive { get; set; }

    DonViChienDau unit;

    public void CancelShooter()
    {
        if (sung1)
        {
            sung1.StopAllCoroutines();
        }
        if (sung2)
        {
            sung2.StopAllCoroutines();
        }
        if (sung3)
        {
            sung3.StopAllCoroutines();
        }
        if (sung4)
        {
            sung4.StopAllCoroutines();
        }
    }

    public bool FireAt(Vector3 target, bool shouldPlayAtkAnim = true)
    {
        bool isFired = false;
        if (sung1)
        {
            isFired = sung1.FireAt(target);
        }
        if (unit == null) unit = GetComponentInParent<DonViChienDau>();

        if (unit.GetBuffCount(BuffType.SIDE_SHOT) > 0)
        {
            if (sung2)
            {
                sung2.DMG = sung1.DMG;
                sung2.ProjSpd = sung1.ProjSpd;
                sung2.AtkSpd = sung1.AtkSpd;
                sung2.AttackAnimTime = sung1.AttackAnimTime;
                sung2.DelayActiveDamage = sung1.DelayActiveDamage;
                sung2.KnockBackDistance = sung1.KnockBackDistance;
                sung2.FireAt(target);
            }
        }

        if (unit.GetBuffCount(BuffType.CHEO_SHOT) > 0)
        {
            if (sung3)
            {
                sung3.DMG = sung1.DMG;
                sung3.ProjSpd = sung1.ProjSpd;
                sung3.AtkSpd = sung1.AtkSpd;
                sung3.AttackAnimTime = sung1.AttackAnimTime;
                sung3.DelayActiveDamage = sung1.DelayActiveDamage;
                sung3.KnockBackDistance = sung1.KnockBackDistance;
                sung3.FireAt(target);
            }
        }

        if (unit.GetBuffCount(BuffType.BACK_SHOT) > 0)
        {
            if (sung4)
            {
                sung4.DMG = sung1.DMG;
                sung4.ProjSpd = sung1.ProjSpd;
                sung4.AtkSpd = sung1.AtkSpd;
                sung4.AttackAnimTime = sung1.AttackAnimTime;
                sung4.DelayActiveDamage = sung1.DelayActiveDamage;
                sung4.KnockBackDistance = sung1.KnockBackDistance;
                sung4.FireAt(target);
            }
        }

        if (isFired && shouldPlayAtkAnim)
        {
            if (unit.Animators != null && unit.Animators.Length > 0)
            {
                //if (mAgent)
                {
                    for (int i = 0; i < unit.Animators.Length; ++i)
                    {
                        var anim = unit.Animators[i];
                        if (anim)
                        {
                            var trigger = string.IsNullOrEmpty(animTrigger) ? "atk" : animTrigger;
                            anim.SetTrigger(trigger);
                            //Debug.Log(gameObject.name + "SetTriggerAtk " + Time.time);
                            anim.SetFloat("atkSpd", sung1.AtkSpdAnimScale);
                        }
                    }
                }
            }
        }

        return isFired;
    }
}
