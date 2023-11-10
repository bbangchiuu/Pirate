using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAura : MonoBehaviour
{
    protected float interval = 1f;
    protected float range = 1f;

    protected List<EffectConfig> listEffect = new List<EffectConfig>();

    protected DonViChienDau unit;
    protected float mTimeCount = 0;

    public float GetRange() { return range; }

    private void Awake()
    {
        unit = GetComponent<DonViChienDau>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected virtual void OnAuraExecute()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (interval <= 0 || mTimeCount >= interval)
        {
            mTimeCount = 0;

            OnAuraExecute();
        }

        mTimeCount += Time.deltaTime;
    }
}
