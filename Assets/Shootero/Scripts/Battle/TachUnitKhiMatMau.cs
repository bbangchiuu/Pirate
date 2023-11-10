using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TachUnitKhiMatMau : MonoBehaviour
{
    public GameObject originPrefab;
    public string prefabResPath;

    public float[] HeSoMatMau;
    public float[] ScaleRate;
    public int SoUnitTach = 2;
    public int SoLanTach = 1;
    public bool IsUnitGoc { get; set; }

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
    }

    void OnUnitMatMau(long dmg)
    {
        if (SoLanTach <= 0) return;
        var hesoMau = unit.GetCurHP() * 100f / unit.GetMaxHP();
        if (hesoMau < 100 - HeSoMatMau[SoLanTach - 1])
        {
            TachUnit();
        }
    }

    void TachUnit()
    {
        if (SoLanTach <= 0) return;
        var reMainHP = unit.GetCurHP();
        var remainMaxHP = unit.GetMaxHP();
        if (reMainHP <= 0 || remainMaxHP <= 0)
        {
            return;
        }

        //if (QuanlyNguoichoi.Instance.PlayerUnit.LastTarget == unit)
        {
            QuanlyNguoichoi.Instance.PlayerUnit.SetTarget(null);
        }

        for (int i = SoUnitTach; i > 0; --i)
        {
            var mauTach = reMainHP / i;
            var maxMauTach = remainMaxHP / i;
            reMainHP -= mauTach;
            remainMaxHP -= maxMauTach;

            var r = Random.insideUnitCircle * 3;
            var dr = new Vector3(r.x, 0, r.y);
            var pos = transform.position + dr;

            GameObject originObj = null;
            if (string.IsNullOrEmpty(prefabResPath) == false)
            {
                originObj = Resources.Load<GameObject>(prefabResPath);
            }
            if (originObj == null && originPrefab != null)
            {
                originObj = originPrefab;
            }
            if (originObj == null) originObj = gameObject;

            var newGo = Instantiate(originObj, pos, Quaternion.LookRotation(dr));
            var tachBehavior = newGo.GetComponent<TachUnitKhiMatMau>();
            tachBehavior.SoLanTach = SoLanTach - 1;
            var scale = ScaleRate[SoLanTach - 1];
            newGo.transform.localScale = Vector3.one * scale;
            var newUnit = newGo.GetComponent<DonViChienDau>();

            newUnit.OverrideCurHPBar(Hiker.GUI.Shootero.ScreenBattle.instance.CreateHPBar(newUnit, false));
            newUnit.OverrideChiSoMau(mauTach, maxMauTach);
        }
        SoLanTach = 0;
        SoUnitTach = 0;
        unit.OnUnitTakenDMG -= OnUnitMatMau;
        IsUnitGoc = true;

        QuanlyManchoi.RemoveEnemyUnit(unit);

        //gameObject.SetActive(false);
        //Destroy(gameObject);
    }
}
