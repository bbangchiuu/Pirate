using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollower : MonoBehaviour
{
    public Transform target;

    Vector3 dif;

    public bool followHorizontal = true;
    public bool followVertical = true;

    public bool followRotation = false;
    public float followRotationSpd = 4f;

    bool inited = false;

    // Start is called before the first frame update
    void Start()
    {
        inited = false;
        if (inited == false)
        {
            InitPos();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inited && target != null)
        {
            var newPos = target.transform.position + dif;
            if (followHorizontal == false)
            {
                newPos.x = transform.position.x;
            }
            if (followVertical == false)
            {
                newPos.z = transform.position.z;
            }

            transform.position = newPos;

            if (followRotation)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * followRotationSpd);
            }
        }
    }

    public void InitPos()
    {
        if (target != null)
        {
            dif = transform.position - target.transform.position;
            inited = true;

            if (enabled == false)
            {
                enabled = true;
            }
        }
    }
}
