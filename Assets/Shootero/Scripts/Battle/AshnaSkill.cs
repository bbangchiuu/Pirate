using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AshnaSkill : MonoBehaviour
{
    [System.Serializable]
    public class PopoBienHinhConfig
    {
        public SungCls Sung1;
        public SungCls Sung2;
        public float MoveSpeed;
        public float HeSoMatMau;
    }

    public PopoBienHinhConfig[] LuotBienHinh;

    public GameObject BienHinhEff;
    public AudioSource BienHinhSound;
    
    //public float[] ScaleRate;
    //public int SoUnitTach = 2;
    public int SoLanTach = 1;
    public float timeDelay = 5f;
    public bool IsUnitGoc { get; set; }

    List<string> listUnit;

    DonViChienDau unit;
    private void Awake()
    {
        unit = GetComponent<DonViChienDau>();
        unit.OnUnitTakenDMG += OnUnitMatMau;
        IsUnitGoc = false;
    }

    private void OnDestroy()
    {
        if (unit)
            unit.OnUnitTakenDMG -= OnUnitMatMau;

        if (listUnit != null)
        {
            Hiker.Util.ListPool<string>.Release(listUnit);
            listUnit = null;
        }
    }

    IEnumerator GamRuBienHinh()
    {
        GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().DisableBehavior();

        yield return new WaitForSeconds(0.5f);
        unit.Animators[0].SetTrigger("skill");

        if (BienHinhEff)
        {
            BienHinhEff.SetActive(true);
            BienHinhSound.Play();
        }

        yield return new WaitForSeconds(1f);

        if (BienHinhEff)
            BienHinhEff.SetActive(false);

        GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().EnableBehavior();
    }

    void OnUnitMatMau(long dmg)
    {
        if (SoLanTach <= 0) return;
        var hesoMau = unit.GetCurHP() * 100f / unit.GetMaxHP();
        var cfg = LuotBienHinh[SoLanTach - 1];
        
        if (hesoMau < 100 - cfg.HeSoMatMau)
        {
            SoLanTach = SoLanTach - 1;

            unit.shooter.SetBarels( cfg.Sung1.Barels);
            unit.shooter2.SetBarels(cfg.Sung2.Barels);
            //unit.UpdateShooterStat();
            unit.UpdateMoveSpeedToCurStat(cfg.MoveSpeed);

            StartCoroutine("GamRuBienHinh");
        }
    }

}
