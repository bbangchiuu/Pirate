using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PopoDenSkill : MonoBehaviour
{
    [System.Serializable]
    public class PopoBienHinhConfig
    {
        public GameObject[] prefabs;
        public float HeSoMatMau;
    }

    public PopoBienHinhConfig[] LuotBienHinh;
    
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

    void OnUnitMatMau(long dmg)
    {
        if (SoLanTach <= 0) return;
        var hesoMau = unit.GetCurHP() * 100f / unit.GetMaxHP();
        var cfg = LuotBienHinh[SoLanTach - 1];
        if (hesoMau < 100 - cfg.HeSoMatMau)
        {
            if (unit.UnitName != "PopoDen")
            {
                InstantiatePopo();
            }
            else
            {
                TachUnit();
            }
        }
    }

    void InstantiatePopo()
    {
        var reMainHP = unit.GetCurHP();
        var remainMaxHP = unit.GetMaxHP();
        var mauTach = reMainHP;
        var maxMauTach = remainMaxHP;

        var originObj = Resources.Load<GameObject>("AIPrefabs/PopoDen");
        var pos = transform.position;
        if (unit.GetAgent() == null || unit.GetAgent().isOnNavMesh == false)
        {
            if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 0.5f, NavMesh.AllAreas) == false)
            {
                pos.z = 0;
                if (NavMesh.SamplePosition(pos, out hit, 0.5f, NavMesh.AllAreas))
                {
                    pos = hit.position;
                }
            }
        }
        
        var newGo = Instantiate(originObj, pos, transform.rotation);

        var tachBehavior = newGo.AddMissingComponent<PopoDenSkill>();
        tachBehavior.SoLanTach = SoLanTach;
        tachBehavior.LuotBienHinh = LuotBienHinh;

        tachBehavior.listUnit = Hiker.Util.ListPool<string>.Claim();
        tachBehavior.listUnit.AddRange(listUnit);
        tachBehavior.timeDelay = timeDelay;
        //var scale = ScaleRate[SoLanTach - 1];
        //newGo.transform.localScale = Vector3.one * scale;
        var newUnit = newGo.GetComponent<DonViChienDau>();

        //newUnit.OverrideCurHPBar(Hiker.GUI.Shootero.ScreenBattle.instance.CreateHPBar(newUnit, false));
        newUnit.OverrideChiSoMau(mauTach, maxMauTach);

        SoLanTach = 0;
        //SoUnitTach = 0;
        unit.DontHaveDrop = true;
        unit.OnUnitTakenDMG -= OnUnitMatMau;
        IsUnitGoc = true;

        QuanlyManchoi.RemoveEnemyUnit(unit);
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

        int remainTach = SoLanTach - 1;
        SoLanTach = 0;
        Hiker.HikerUtils.DoAction(this, () =>
        {
            reMainHP = unit.GetCurHP();
            remainMaxHP = unit.GetMaxHP();
            if (reMainHP <= 0 || remainMaxHP <= 0)
            {
                return;
            }
            var mauTach = reMainHP;
            var maxMauTach = remainMaxHP;
            reMainHP -= mauTach;
            remainMaxHP -= maxMauTach;

            //var r = Random.insideUnitCircle * 3;
            //var dr = new Vector3(r.x, 0, r.y);
            var pos = transform.position;// + dr;

            if (listUnit == null) listUnit = Hiker.Util.ListPool<string>.Claim();
            List<GameObject> listPrefabs = Hiker.Util.ListPool<GameObject>.Claim();
            var cfg = LuotBienHinh[remainTach];

            foreach (var s in cfg.prefabs)
            {
                if (s != null && listUnit.Contains(s.name) == false)
                {
                    listPrefabs.Add(s);
                }
            }

            GameObject originObj = listPrefabs[Random.Range(0, listPrefabs.Count)];

            Hiker.Util.ListPool<GameObject>.Release(listPrefabs);
            //if (string.IsNullOrEmpty(prefabResPath) == false)
            //{
            //    originObj = Resources.Load<GameObject>(prefabResPath);
            //}
            //if (originObj == null && originPrefab != null)
            //{
            //    originObj = originPrefab;
            //}
            //if (originObj == null) originObj = gameObject;
            listUnit.Add(originObj.name);

            var newGo = Instantiate(originObj, pos, transform.rotation);

            var tachBehavior = newGo.AddMissingComponent<PopoDenSkill>();
            tachBehavior.SoLanTach = remainTach;
            tachBehavior.LuotBienHinh = LuotBienHinh;
            tachBehavior.timeDelay = timeDelay;

            tachBehavior.listUnit = Hiker.Util.ListPool<string>.Claim();
            tachBehavior.listUnit.AddRange(listUnit);

            //var scale = ScaleRate[SoLanTach - 1];
            //newGo.transform.localScale = Vector3.one * scale;
            var newUnit = newGo.GetComponent<DonViChienDau>();

            //newUnit.OverrideCurHPBar(Hiker.GUI.Shootero.ScreenBattle.instance.CreateHPBar(newUnit, false));
            newUnit.OverrideChiSoMau(mauTach, maxMauTach);

            //SoUnitTach = 0;
            unit.DontHaveDrop = true;
            unit.OnUnitTakenDMG -= OnUnitMatMau;
            IsUnitGoc = true;

            QuanlyManchoi.RemoveEnemyUnit(unit);
        }, timeDelay, false);
    }
}
