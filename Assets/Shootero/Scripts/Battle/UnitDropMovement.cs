using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDropMovement : MonoBehaviour
{
    public Vector3 targetPosition;
    public float Speed;

    public float RandomRadius = 0;

    public float maxHeight = 3;

    public float initTime = 0.1f;

    public Vector3 Target { get; protected set; }
    
    GameObject mIndicator;
    bool mIsActive = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    protected void Update()
    {
        if (mIsActive)
        {
            //transform.Translate(Speed * transform.forward * Time.deltaTime, Space.World);
            UpdateDropMovement(Time.unscaledDeltaTime);
        }

        if (QuanlyNguoichoi.Instance.IsLoadingMission)
        {
            gameObject.SetActive(false);
            ObjectPoolManager.Unspawn(gameObject);
        }
    }

    float initTimeClock = 0;
    void UpdateDropMovement(float deltaTime)
    {
        float crDistance = Vector3.Distance(this.transform.position, Target);
        Vector3 dirToTarget = (Target - this.transform.position).normalized;
        //dirToTarget.y = 0;
        Vector3 dirToUp = Vector3.RotateTowards(Vector3.up, dirToTarget, 35 * Mathf.Deg2Rad, 0).normalized;
        Vector3 dir = Vector3.zero;
        initTimeClock += deltaTime;
        float ratio = initTimeClock / initTime;
        ratio = Mathf.Clamp01(ratio);

        dir = Vector3.Slerp(dirToUp, dirToTarget, ratio);

        if (dir == Vector3.zero)
        {
            this.transform.LookAt(Target);
        }
        else
        {
            this.transform.LookAt(dir);
        }

        float moveDistance = Speed * deltaTime;

        if (crDistance < moveDistance) moveDistance = crDistance;
        Vector3 moveSpeed = dir.normalized * moveDistance;

        this.transform.position += moveSpeed;

        if (this.transform.position.y <= Target.y || crDistance <= 0.01f)
        {
            //UnityEditor.EditorApplication.isPaused = true;
            this.transform.position = new Vector3 (transform.position.x, Target.y, transform.position.z);
            mIsActive = false;
            //enabled = false;
            //gameObject.SetActive(false);
            //ObjectPoolManager.Unspawn(gameObject);
        }
    }

    public void Activate(float speed, Vector3 target)
    {
        if (enabled == false) enabled = true;

        this.Speed = speed;
        targetPosition = target;
        initTimeClock = 0;
        if (RandomRadius > 0)
        {
            var r = Random.insideUnitCircle * RandomRadius;

            this.Target = targetPosition + new Vector3(r.x, 0, r.y);
        }
        else
        {
            this.Target = targetPosition;
        }

        var t = Vector3.Distance(transform.position, Target) / speed;
        initTime = Mathf.Max(t * 0.5f, 0.1f);
        mIsActive = true;
    }
}
