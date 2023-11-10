using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaiTuTeLaser : MonoBehaviour
{
    public GameObject Target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (QuanlyManchoi.instance != null && QuanlyManchoi.instance.IsLevelClear())
        {
            this.gameObject.SetActive(false);
        }

        if (Target != null)
        {
            this.transform.LookAt(Target.transform.position + Vector3.up);

            float dis = (Target.transform.position - this.transform.position).magnitude;
            this.transform.localScale = new Vector3(1,1,dis);
        }
    }
}
