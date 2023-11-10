using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongChimCircle : DanBay
{
    public LongChimShaman prefabLongChim;
    public Transform[] longChimAnchors;
    public TweenScale tweenScale;
    public TweenRotation tweenRotation;
    LongChimShaman[] longChim;

    public override void ActiveDan(float speed, long dmg, Vector3 target)
    {
        var curStat = SourceUnit.GetCurStat();
        base.ActiveDan(speed, dmg, target);
        tweenScale.ResetToBeginning();
        tweenScale.enabled = true;
        tweenRotation.ResetToBeginning();
        if (longChim == null || longChim.Length != longChimAnchors.Length)
        {
            longChim = new LongChimShaman[longChimAnchors.Length];
        }
        
        for (int i = longChim.Length - 1; i >= 0; --i)
        {
            if (longChim[i] == null)
            {
                longChim[i] = Instantiate(prefabLongChim, 
                    longChimAnchors[i].transform.position,
                    longChimAnchors[i].rotation,
                    transform);
            }
            if (longChim[i])
            {
                longChim[i].SourceUnit = SourceUnit;
                longChim[i].transform.SetParent(transform);
                longChim[i].transform.rotation = longChimAnchors[i].transform.rotation;
                longChim[i].transform.position = longChimAnchors[i].transform.position;
                longChim[i].ActiveDan(curStat.PROJ_SPD, dmg, target);
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        if (longChim != null)
        {
            bool isFowardPhase = false;
            for (int i = longChim.Length - 1; i >= 0; --i)
            {
                if (longChim[i] && longChim[i].IsForwardPhase())
                {
                    longChim[i].transform.position = longChimAnchors[i].transform.position;
                    longChim[i].transform.rotation = longChimAnchors[i].transform.rotation;

                    isFowardPhase = true;
                }
            }
            if (isFowardPhase == false)
            {
                DeactiveProj();
            }
        }
        else
        {
            DeactiveProj();
        }
    }

    protected override void OnDeactiveProjectile()
    {
        if (longChim != null)
        {
            for (int i = longChim.Length - 1; i >= 0; --i)
            {
                if (longChim[i])
                {
                    longChim[i].transform.SetParent(null, true);

                    longChim[i] = null;
                }
            }
        }
        
        base.OnDeactiveProjectile();
    }
}
