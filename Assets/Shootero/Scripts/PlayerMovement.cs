using Hiker.GUI.Shootero;
using UnityEngine;
using UnityEngine.AI;
using UnitySampleAssets.CrossPlatformInput;


public class PlayerMovement : MonoBehaviour
{
    Vector3 movement;                   // The vector to store the direction of the player's movement.
    DonViChienDau unit;
    //Animator anim;                      // Reference to the animator component.
    public NavMeshAgent characterController;          // Reference to the player's rigidbody.
                                            //#if !MOBILE_INPUT
                                            //        int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
                                            //        float camRayLength = 100f;          // The length of the ray from the camera into the scene.
                                            //#endif
    public Joystick joystick;

    public bool vuaDiVuaBan = false;

    //PlayerShooting gun;
    PlayerVisual playerVisual;

    void Awake()
    {
        //#if !MOBILE_INPUT
        //            // Create a layer mask for the floor layer.
        //            floorMask = LayerMask.GetMask("Floor");
        //#endif

        //// Set up references.
        //anim = GetComponent<Animator>();
        unit = GetComponent<DonViChienDau>();
        characterController = GetComponent<NavMeshAgent>();
        characterController.updateRotation = false;
        //gun = GetComponentInChildren<PlayerShooting>();
        playerVisual = GetComponentInChildren<PlayerVisual>();
        Physics.autoSyncTransforms = false;
    }

    /// <summary>
    /// Store the input axes.
    /// </summary>
    void UpdateDirection()
    {
        if ((PopupAngelBuff.instance && PopupAngelBuff.instance.gameObject.activeInHierarchy) ||
            (PopupBattleSecretShop.instance && PopupBattleSecretShop.instance.gameObject.activeInHierarchy) ||
            (PopupBlackSmith.instance && PopupBlackSmith.instance.gameObject.activeInHierarchy) ||
            (PopupDevilBuff.instance && PopupDevilBuff.instance.gameObject.activeInHierarchy) ||
            (PopupDungeonSelect.instance && PopupDungeonSelect.instance.gameObject.activeInHierarchy) ||
            (PopupFightingNPC.instance && PopupFightingNPC.instance.gameObject.activeInHierarchy) ||
            (PopupGreedyGoblin.instance && PopupGreedyGoblin.instance.gameObject.activeInHierarchy) ||
            (PopupOpenChest.instance && PopupOpenChest.instance.gameObject.activeInHierarchy) ||
            (PopupTrap.instance && PopupTrap.instance.gameObject.activeInHierarchy) ||
            (PopupBattlePause.instance && PopupBattlePause.instance.gameObject.activeInHierarchy))
        {
            LastDirection = Vector2.zero;
            return;
        }

        float h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        float v = CrossPlatformInputManager.GetAxisRaw("Vertical");

#if UNITY_STANDALONE
        if (joystick == null)
        {
            if (ScreenBattle.instance != null)
            {
                joystick = ScreenBattle.instance.joystick;
            }
        }

        if (joystick != null &&
            joystick.Direction != Vector2.zero)
        {
            h = joystick.Horizontal;
            v = joystick.Vertical;
        }
#endif

        LastDirection = new Vector2(h, v);
    }

    private void Update()
    {
        if (QuanlyNguoichoi.Instance == null) return;
        if (QuanlyNguoichoi.Instance.IsLoadingMission) return;
        if (QuanlyManchoi.instance == null) return;
        if (unit && unit.IsAlive() == false) return;

        UpdateDirection();
        var h = LastDirection.x;
        var v = LastDirection.y;
        if (playerVisual == null) playerVisual = GetComponentInChildren<PlayerVisual>();

        if (playerVisual != null)
            playerVisual.SetMoving(h != 0 || v != 0);

        if (h != 0 || v != 0)
        {
            if (vuaDiVuaBan == false)
                unit.CancelShooter();

            // Move the player around the scene.
            Move(h, v, Time.deltaTime);


            //if (gun.lockTarget == null)
            {
                // Turn the player to face the mouse cursor.
                Turning(movement);
            }
        }

        // Animate the player.
        Animating(h, v);

        //if (LevelController.instance == null)
        //{
        //    characterController.enabled = false;
        //}
        //else if (characterController.enabled == false)
        //{
        //    characterController.enabled = true;
        //}
        
        // var pos = characterController.transform.position;
        // if (//characterController.isGrounded == false &&
        //     pos.y < -0.1f || pos.y > 0.5f)
        // {
        //     pos.y = 0.2f;
        //     characterController.transform.position = pos;
        // }
    }

    public Vector2 LastDirection { get; private set; }

    //void FixedUpdate()
    //{
    //    if (LevelController.instance == null) return;
    //    if (unit && unit.IsAlive() == false) return;

    //    UpdateDirection();

    //    // Store the input axes.
    //    var h = LastDirection.x;
    //    var v = LastDirection.y;

    //    if (h != 0 || v != 0)
    //    {
    //        // Move the player around the scene.
    //        Move(h, v);

    //        //if (gun.lockTarget == null)
    //        {
    //            // Turn the player to face the mouse cursor.
    //            Turning(movement);
    //        }
    //    }
    //}


    void Move(float h, float v, float deltaTime)
    {
        // Set the movement vector based on the axis input.
        movement.Set(h, 0, v);

        // Normalise the movement vector and make it proportional to the speed per second.
        movement = movement.normalized * unit.GetStat().SPD;

        // khong can check vi da co collider chan duong
        //if (MapController.instance.CheckPointInMap(transform.position + movement * deltaTime) == false) // if out of map then no move
        //{
        //    movement.Set(0, 0, 0);
        //}

        // Move the player to it's current position plus the movement.
        if (characterController.isOnNavMesh)
        {
            characterController.Move(movement * deltaTime);
        }
        else
        {
            Debug.Log("Not on navmesh");
        }//var flag = characterController.Move(movement * deltaTime);
    }

    public void Turning(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0)
        {
            if (vuaDiVuaBan == false ||
                unit.LastTarget == null)
            {
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }
    }


    void Animating(float h, float v)
    {
        // Create a boolean that is true if either of the input axes is non-zero.
        //bool walking = h != 0f || v != 0f;

        // Tell the animator whether or not the player is walking.
        //anim.SetBool("IsWalking", walking);
    }
}