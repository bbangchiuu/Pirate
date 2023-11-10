using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (QuanlyNguoichoi.Instance.IsLevelClear)
        {
            OnCollisionWithProjectile();
            return;
        }
    }

    public void OnCollisionWithProjectile()
    {
        GameObject.Destroy(this.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.layer == LayerMask.GetMask("Team2_Dan"))
        {
            OnCollisionWithProjectile();
        }
    }

}
