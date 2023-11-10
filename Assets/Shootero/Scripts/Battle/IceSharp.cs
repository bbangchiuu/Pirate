using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSharp : MonoBehaviour
{
    public SatThuongDT SpawnOnDestroy = null;
    public float SpawnObjSpeed = 10;

    public Bounds MetoerRandomField;
    public AudioSource crackSound;

    public CapsuleCollider CapCollider;
    public GameObject Visual;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 target = new Vector3(
            Random.Range(MetoerRandomField.min.x, MetoerRandomField.max.x),
            0,
            Random.Range(MetoerRandomField.min.z, MetoerRandomField.max.z));

        
        transform.position = target;
        transform.localEulerAngles = new Vector3(0,Random.Range(0,360),0);
    }

    // Update is called once per frame
    void Update()
    {
        if (QuanlyNguoichoi.Instance.IsLevelClear)
        {
            //OnCollisionWithProjectile();
            GameObject.Destroy(this.gameObject);

            return;
        }
    }

    private bool isDestroyed = false;

    public void OnCollisionWithProjectile()
    {
        if (isDestroyed)
            return;

        isDestroyed = true;

        if (SpawnOnDestroy)
        {
            int SpawnOnDestroyCount = 4;

            float rand_y = Random.Range(0, 360);
            float degree_step = 360 / SpawnOnDestroyCount;

            long dmg = 0;
            if (QuanlyNguoichoi.Instance != null)
            {
                dmg = QuanlyNguoichoi.Instance.GetDMGEnemy();
            }
            else
            {
                dmg = 100;
            }

            for (int i = 0; i < SpawnOnDestroyCount; i++)
            {
                var projObj = ObjectPoolManager.Spawn(SpawnOnDestroy.gameObject);
                var proj = projObj.GetComponent<SatThuongDT>(); // Instantiate(SpawnOnDestroy);
                proj.transform.eulerAngles = new Vector3(0, rand_y, 0);
                proj.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
                proj.ActiveDan(SpawnObjSpeed, dmg, Vector3.one);
                proj.gameObject.SetActive(true);

                rand_y += degree_step;
            }

            if(crackSound)
                crackSound.Play();
        }

        Visual.SetActive(false);
        CapCollider.enabled = false;

        GameObject.Destroy(this.gameObject,1);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.layer == LayerMask.GetMask("Team2_Dan"))
        {
            OnCollisionWithProjectile();
        }
    }

}
