using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateVisualController : MonoBehaviour
{
    public static GateVisualController instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject GateClose;
    public GameObject GateOpen;

    public GameObject MissionBoard;
    public GameObject BossBoard;
    public GameObject MissionLlb;

    AudioSource GateOpenSound;

    public void Start()
    {
        GateClose.SetActive(true);
        GateOpen.SetActive(false);

        GateOpenSound = gameObject.AddComponent<AudioSource>();
        GateOpenSound.spatialBlend = 0f;
        GateOpenSound.clip = Resources.Load("Sound/SFX/Battle/door_open") as AudioClip;
    }

    public void OpenGate()
    {
        if (GateClose)
            GateClose.SetActive(false);

        if (GateOpen)
            GateOpen.SetActive(true);

        if (GateOpenSound)
            GateOpenSound.Play();
    }

    public void SetMission( int mission_idx, bool isBoss = false)
    {
        MissionBoard.SetActive(false);
        BossBoard.SetActive(false);

        //if(isBoss)
        //{
        //    MissionBoard.SetActive(false);
        //    BossBoard.SetActive(true);
        //}
        //else
        //{
        //    if (mission_idx > 0)
        //    {
        //        MissionBoard.SetActive(true);
        //        BossBoard.SetActive(false);

        //        MeshRenderer renderer = MissionLlb.GetComponent<MeshRenderer>();
        //        renderer.sortingOrder = 30;

        //        TextMesh textMesh = MissionLlb.GetComponent<TextMesh>();
        //        textMesh.text = mission_idx.ToString();
        //    }
        //    else
        //    {
        //        MissionBoard.SetActive(false);
        //        BossBoard.SetActive(false);
        //    }
        //}
    }
}
