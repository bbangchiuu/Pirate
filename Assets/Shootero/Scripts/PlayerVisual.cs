using Hiker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hiker.GUI;

public class PlayerVisual : MonoBehaviour
{
    public Animator animator;
    public Transform jntWeapon;

    GameObject weaponObj;
    public string WeaponName { get; set; }

    AudioClip[] FootSoundClip;
    AudioClip FireSoundClip;

    //AudioSource FireSoundSrc;
    AudioSource FootSoundSrc;

    int movingParam = Animator.StringToHash("moving");

    public void Start()
    {
        InitSound();
        var mesh = animator.GetComponentInChildren<SkinnedMeshRenderer>();
        mesh.quality = SkinQuality.Bone4;
    }

    public void SetVisualWeapon(bool on)
    {
        weaponObj.SetActive(on);
    }

    bool isInitSound = false;

    public void InitSound()
    {
        if(!isInitSound)
        {
            isInitSound = true;

            int numSound = 7;
            if (FootSoundClip == null || FootSoundClip.Length != numSound)
            {
                FootSoundClip = new AudioClip[numSound];
            }
            
            string clip_path_format = "Sound/SFX/Battle/footstep0{0}";

            for (int i = 0; i < 7; i++)
            {
                string clip_path = string.Format(clip_path_format, i + 1);
                AudioClip clip = Resources.Load<AudioClip>(clip_path);
                FootSoundClip[i] = clip;
            }
            
            FootSoundSrc = gameObject.AddComponent<AudioSource>();
            FootSoundSrc.loop = false;
            FootSoundSrc.volume = 0.15f;
            FootSoundSrc.spatialBlend = 0f;
        }
    }

    bool m_IsMoving = false;

    public void Update()
    {
        if(m_IsMoving)
        {
            UpdateFootStep();
        }

        if (lastTimePlayFootStep > 0)
            lastTimePlayFootStep -= Time.deltaTime;
    }

    float lastTimePlayFootStep = 0;

    public void UpdateFootStep()
    {
        if (!FootSoundSrc.isPlaying && lastTimePlayFootStep <= 0)
        {
            int s_i = Random.Range(0, FootSoundClip.Length);

            FootSoundSrc.clip = FootSoundClip[s_i];
            FootSoundSrc.Play();

            lastTimePlayFootStep = 0.3f;
        }

        if( GUIManager.Instance.CurrentScreen=="Main")
        {
            m_IsMoving = false;
        }
    }

    public GameObject LoadWeapon(string weapon, bool inBattle = true)
    {
        if (weaponObj != null && weaponObj.name != weapon)
        {
            weaponObj.gameObject.SetActive(false);
            Destroy(weaponObj);
            WeaponName = string.Empty;
            weaponObj = null;
        }

        if (string.IsNullOrEmpty(weapon) == false && weaponObj == null)
        {
            weaponObj = Instantiate(Resources.Load<GameObject>("Weapons/" + weapon), jntWeapon);
            weaponObj.name = weapon;
            weaponObj.transform.localPosition = Vector3.zero;
            weaponObj.transform.localRotation = Quaternion.identity;
            weaponObj.transform.SetLayer(gameObject.layer, true);
            WeaponName = weapon;
        }


        InitSound();

        string fire_sound_name = "main_attack_melee";

        var cfg = ConfigManager.GetItemConfig(weapon);
        if (cfg != null)
        {
            if (cfg.VuKhiType == "Gun")
            {
                fire_sound_name = "main_attack_gun";
            }
            else
                fire_sound_name = "main_attack_melee";
        }

        if (inBattle)
        {
            QuanlyNguoichoi.Instance.PlayerUnit.shooter.SetFireClip(fire_sound_name);
        }

        return weaponObj;
    }
    
    public void SetMoving(bool isMoving)
    {
        m_IsMoving = isMoving;
        if (animator)
            animator.SetBool(movingParam, isMoving);
    }
}
