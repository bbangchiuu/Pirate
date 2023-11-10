using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaseLineTank : MonoBehaviour
{
    public LayerMask mask;
    public long Damage;

    LineRenderer line;

    public bool IsBossDamage = false;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
        lastForward = transform.forward;
        tickDamage = 0;
        lastFrameHit = false;
        
        if (mask.value == LayersMan.Team1LegacyHitLayerMask)
        {
            mask.value = LayersMan.Team1HitLayerMask;
        }

        if (QuanlyNguoichoi.Instance &&
            LayersMan.CheckMask(LayersMan.Team1, mask.value))
        {
            Damage = QuanlyNguoichoi.Instance.GetDMGEnemy();
        }
    }

    bool lastFrameHit = false;
    Vector3 lastForward = Vector3.zero;

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100, mask))
        {
            SetTarget(hit.point);
            OnHitPlayer();
        }
        else
        {
            SetTarget(transform.position + transform.forward * 100);
            if (IsPlayerInMidle())
            {
                OnHitPlayer();
            }
            else
            {
                lastFrameHit = false;
            }
        }

        lastForward = transform.forward;
    }

    float tickDamage = 0;
    void OnHitPlayer()
    {
        //Debug.Log("Hit Player");
        if (lastFrameHit == false)
        {
            tickDamage = 0;
        }
        if (tickDamage <= 0)
        {
            QuanlyNguoichoi.Instance.PlayerUnit.TakeDamage(Damage, false, null, true, true,false,false,true,false, IsBossDamage);
            tickDamage = 1;
        }
        lastFrameHit = true;
    }

    bool IsPlayerInMidle()
    {
        var curForward = transform.forward;
        curForward.y = 0;
        lastForward.y = 0;

        var dir = QuanlyNguoichoi.Instance.PlayerUnit.transform.position - transform.position;
        dir.y = 0;

        var totalAngle = Vector3.Angle(lastForward, curForward);
        var angle1 = Vector3.Angle(lastForward, dir);
        var angle2 = Vector3.Angle(dir, curForward);

        //Debug.LogFormat("total angle {0} = {1} + {2}", totalAngle, angle1, angle2);
        if (CompareFloatAproximate(angle1 + angle2, totalAngle))
        {
            return true;
        }
        return false;
    }

    bool CompareFloatAproximate(float a, float b)
    {
        if (Mathf.Abs(a - b) <= float.Epsilon * 2)
        {
            return true;
        }
        return false;
    }

    public void SetTarget(Vector3 p)
    {
        var localPoint = line.transform.InverseTransformPoint(p);
        line.SetPosition(1, localPoint);
    }
}
