using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinShield : MonoBehaviour
{
    public UnitShield[] shields;
    public float rotateSpeed = 180f;

    TargetFollower follower;

    private void OnEnable()
    {
        follower = GetComponent<TargetFollower>();
        transform.position = QuanlyNguoichoi.Instance.PlayerUnit.transform.position;
        follower.target = QuanlyNguoichoi.Instance.PlayerUnit.transform;
        follower.InitPos();

        for (int i = 0; i < shields.Length; ++i)
        {
            if (shields[i])
            {
                shields[i].gameObject.SetActive(true);
            }
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.Self);
    }
}
