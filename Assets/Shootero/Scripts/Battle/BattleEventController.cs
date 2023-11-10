using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ANTICHEAT
using Int64 = CodeStage.AntiCheat.ObscuredTypes.ObscuredLong;
using Int32 = CodeStage.AntiCheat.ObscuredTypes.ObscuredInt;
using Float = CodeStage.AntiCheat.ObscuredTypes.ObscuredFloat;
using Bool = CodeStage.AntiCheat.ObscuredTypes.ObscuredBool;
using ObString = CodeStage.AntiCheat.ObscuredTypes.ObscuredString;
#else
using Int64 = System.Int64;
using Int32 = System.Int32;
using Float = System.Single;
using Bool = System.Boolean;
using ObString = System.String;
#endif

using Hiker.Networks.Data.Shootero;

public class BattleEventController : MonoBehaviour
{
    public GameObject[] Agents;

    public int RoomType { get; set; }
    public static BattleEventController instance;
    public Int32 NumUserGold { get; set; }
    public Int32 NumUserGem { get; set; }
    public List<KeyValuePair<ObString, Int32>> MaterialData { get; set; }

    public StartBattleEventResponse Response { get; private set; }

    private void OnEnable()
    {
        instance = this;
        if (QuanlyNguoichoi.Instance != null &&
            QuanlyNguoichoi.Instance.IsLevelClear)
        {
            gameObject.SetActive(false);
            instance = null;
            return;
        }

        if (GameClient.instance && GameClient.instance.OfflineMode == false)
        {
            //GameClient.instance.RequestStartBattleEventRoom(PlayerManager.Instance.battleData.ID);
            var idx = QuanlyNguoichoi.Instance.battleData.BattleEventIndex;
            var events = QuanlyNguoichoi.Instance.battleData.ListRoomEvents;
            idx = idx % events.Count;
            if (idx < events.Count)
            {
                BattleEventController.instance.GetBattleEventResponse(events[idx]);
                QuanlyNguoichoi.Instance.battleData.BattleEventIndex++;
            }
        }
    }

    public void GetBattleEventResponse(StartBattleEventResponse response)
    {
        Response = response;
        RoomType = (int)response.room;
#if UNITY_EDITOR
        Debug.Log("Active room type " + RoomType + " - " + response.room.ToString());
#endif
        for (int i = 0; i < Agents.Length; ++i)
        {
            if (Agents[i])
            {
                Agents[i].gameObject.SetActive(i == RoomType);
            }
        }

        NumUserGem = 0;
        NumUserGold = 0;
        if (MaterialData != null) MaterialData.Clear();

        if (response.Rewards != null)
        {
            foreach (var reward in response.Rewards)
            {
                if (reward.k == CardReward.GEM_CARD)
                {
                    NumUserGem += reward.v;
                }
                else if (reward.k == CardReward.GOLD_CARD)
                {
                    NumUserGold += reward.v;
                }
                else if (ConfigManager.Materials.ContainsKey(reward.k))
                {
                    if (MaterialData == null)
                    {
                        MaterialData = new List<KeyValuePair<ObString, Int32>>();
                    }

                    MaterialData.Add(new KeyValuePair<ObString, Int32>(reward.k, reward.v));
                }
            }
        }
    }

    private void Start()
    {
//#if UNITY_EDITOR
//        if (PlayerManager.Instance.TestMission)
//        {
//            var fightNPC = GetComponentInParent<FightingNPC>();
//            if (fightNPC.name.StartsWith("GemNPC"))
//            {
//                NumUserGem = 9;
//            }
//            else if (fightNPC.name.StartsWith("GoldNPC"))
//            {
//                NumUserGold = 121;
//            }
//            else if (fightNPC.name.StartsWith("MaterialNPC"))
//            {
//                MaterialData = new Dictionary<ObString, Int32>();
//                MaterialData.Add(ConfigManager.RandomMaterial(), Random.Range(2, 8));
//            }
//        }
//#endif
    }
}
