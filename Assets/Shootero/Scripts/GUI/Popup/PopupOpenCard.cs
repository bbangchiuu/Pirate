using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    public class PopupOpenCard : PopupBase
    {
        public Text lbItemName;
        public Text lbDesc;
        
        public CardAvatar cardAvatar;
        public Text lbRarity;

        public Animator[] ChestList;

        Animator Chest;
        public GameObject ChestEff;
        public GameObject RewardGroup;

        public GameObject[] ItemEff;

        public AudioSource sound;

        public static PopupOpenCard instance;

        CardReward mReward;

        string[] rewardsItems;
        int mCurIndex = 0;
        int mCurQuantityNum = 0;

        //string m_ChestName;

        public static PopupOpenCard Create(CardReward itemData)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }            

            var go = PopupManager.instance.GetPopup("PopupOpenCard");
            instance = go.GetComponent<PopupOpenCard>();
            instance.Init(itemData);
            return instance;
        }

        private void Init(CardReward reward)
        {
            mReward = reward;
            rewardsItems = new string[reward.Keys.Count];
            int i = 0;

            for (int c = 0; c < ChestList.Length; c++)
                ChestList[c].gameObject.SetActive(false);

            //if ( chest_name== "Chest_Rare")
            //{
            //    Chest = ChestList[0];
            //}

            //if (chest_name == "Chest_Epic")
            //{
            //    Chest = ChestList[1];
            //}

            //if (chest_name == "Chest_Legend")
            //{
            //    Chest = ChestList[2];
            //}
            //Chest = ChestList[2];

            foreach (var s in reward.Keys)
            {
                rewardsItems[i++] = s;
            }
            if (rewardsItems.Length > 0)
            {
                Debug.Log("StartCoroutine(OpenChest)");
                StartCoroutine("OpenChest");
                //ShowReward(0, 0);
            }
            else
            {
                OnTouchClick();
            }
        }


        bool IsCanClose = false;

        [GUIDelegate]
        IEnumerator OpenChest()
        {
            RewardGroup.SetActive(false);
            //ChestEff.SetActive(false);
            //Chest.gameObject.SetActive(true);
            ShowReward(0, 0);

            yield return new WaitForSeconds(0.5f);
            RewardGroup.SetActive(true);

            if (GUIManager.Instance.SoundEnable)
            {
                sound.Play();
            }

            var itemName = rewardsItems[0];

            var rarity = ConfigManager.CardStats[itemName].Rarity;

            if( ItemEff[(int)(rarity)] !=null)
            {
                ItemEff[(int)(rarity)].SetActive(true);
            }


            //ChestEff.SetActive(true);

            //Chest.SetTrigger("open");
            yield return new WaitForSeconds(1f);

            cardAvatar.PlayFlip();

            yield return new WaitForSeconds(1f);
            lbRarity.gameObject.SetActive(true);
            lbItemName.gameObject.SetActive(true);
            //lbDesc.gameObject.SetActive(true);
            IsCanClose = true;

        }

        void ShowReward(int index, int quantity)
        {
            mCurIndex = index;

            var itemName = rewardsItems[index];

            if (quantity == 0)
            {
                quantity = mReward[itemName];
            }
            if (quantity == 0)
            {
                OnTouchClick();
                return;
            }

            mCurQuantityNum = 0;

            
            mCurQuantityNum = quantity;
            var rarity = ConfigManager.CardStats[itemName].Rarity;
            lbRarity.text = Localization.Get(string.Format("Rariry_" + rarity.ToString()));
            cardAvatar.SetItem(itemName, rarity);
            cardAvatar.IsBack = true;

            lbItemName.text = ConfigManager.GetDisplayPhanThuong(itemName);
            //lbDesc.text = ConfigManager.GetMoTaPhanThuong(itemName);

            lbRarity.gameObject.SetActive(false);
            lbItemName.gameObject.SetActive(false);
            //lbDesc.gameObject.SetActive(false);
        }
        [GUIDelegate]
        public void OnTouchClick()
        {
            int remain = mCurQuantityNum - 1;
            if (remain > 0)
            {
                ShowReward(mCurIndex, remain);
            }
            else
            {
                if (mCurIndex < rewardsItems.Length - 1)
                {
                    ShowReward(mCurIndex + 1, 0);
                }
                else
                {
                    OnCloseBtnClick();
                }
            }
        }
        [GUIDelegate]
        public override void OnCloseBtnClick()
        {
            if (!IsCanClose)
                return;

            base.OnCloseBtnClick();

            HikerUtils.DoAction(GameClient.instance, () => GameClient.instance.CheckRewardInfo(), 0.5f, true);

            if (Hiker.GUI.Shootero.PopupTheQuaiVat.instance)
            {
                Hiker.GUI.Shootero.PopupTheQuaiVat.instance.SyncNetworkData();
            }
        }
    }
}