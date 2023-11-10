using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KhongLoHoaModifier : MonoBehaviour
{
    bool inited = false;
    Vector3 originalScale;
    DonViChienDau unit;
    private void Awake()
    {
        if (inited == false)
        {
            originalScale = transform.localScale;
            inited = true;
        }
        unit = GetComponent<DonViChienDau>();
    }

    public float GetTileKhongLo()
    {
#if UNITY_EDITOR
        int tile = Mathf.Max(150, 100);
#else
        int tile = Mathf.Max(ConfigManager.LeoThapCfg.TileKhongLo, 100);
#endif
        return tile / 100f; // default is 1.5f
    }

    public Vector3 GetScaleKhongLo()
    {
        return originalScale * GetTileKhongLo();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (unit != null && unit.transform.parent != null)
        {
            var parent = unit.transform.parent.GetComponent<DonViChienDau>();
            if (parent != null && parent.GetKhongLoHoa() != null)
            {
                return;
            }
        }
        transform.localScale = GetScaleKhongLo();
    }
}
