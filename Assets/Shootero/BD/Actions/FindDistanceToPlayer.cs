using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

[TaskDescription("Caculate distance to player")]
[TaskCategory("Shootero")]
public class FindDistanceToPlayer : Action
{
    public SharedFloat curDistance = 0;
    DonViChienDau unit;

    public override void OnAwake()
    {
        unit = gameObject.GetComponent<DonViChienDau>();
    }

    public override TaskStatus OnUpdate()
    {
        if (QuanlyManchoi.instance == null) return TaskStatus.Failure;

        var player = QuanlyManchoi.instance.PlayerUnit;
        if (player == null) return TaskStatus.Failure;

        var playerBodyRange = player.GetStat().BODY_RADIUS;
        curDistance.SetValue(Vector3.Distance(transform.position, player.transform.position));

        return TaskStatus.Success;
    }
}
