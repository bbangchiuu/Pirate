using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public DonViChienDau playerUnit;

    // Start is called before the first frame update
    void Start()
    {
        var follower = gameObject.AddMissingComponent<TargetFollower>();
        follower.target = playerUnit.transform;
        follower.followHorizontal = true;
        follower.followVertical = true;
        transform.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
