using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    public class PopupBuyChest : PopupBase
    {
        public Text lbItemName;
        public Text lbDesc;
        
        public ItemAvatar avatar;
        public Text lbRarity;

        public MaterialAvatar matAva;

        public Transform GemAvatar;
        public Transform GoldAvatar;

        public Animator[] ChestList;

        Animator Chest;
        public GameObject ChestEff;
        public GameObject RewardGroup;

        public GameObject[] ItemEff;

        public AudioSource sound;

        public static PopupBuyChest instance;

        CardReward mReward;

        string[] rewardsItems;
        int mCurIndex = 0;
        int mCurQuantityNum = 0;

        //string m_ChestName;

        public static PopupBuyChest Create(CardReward itemData,string chest_name)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }            

            var go = PopupManager.instance.GetPopup("PopupBuyChest");
            instance = go.GetComponent<PopupBuyChest>();
            instance.Init(itemData, chest_name);
            return instance;
        }

        private void Init(CardReward reward, string chest_name)
        {
            mReward = reward;
            rewardsItems = new string[reward.Keys.Count];
            int i = 0;

            for (int c = 0; c < ChestList.Length; c++)
                ChestList[c].gameObject.SetActive(false);

            if ( chest_name== "Chest_Rare")
            {
                Chest = ChestList[0];
            }

            if (chest_name == "Chest_Epic")
            {
                Chest = ChestList[1];
            }

            if (chest_name == "Chest_Legend")
            {
                Chest = ChestList[2];
            }

            foreach (var s in reward.Keys)
            {
                rewardsItems[i++] = s;
            }
            if (rewardsItems.Length > 0)
            {
#if UNITY_EDITOR
                Debug.Log("StartCoroutine(OpenChest)");
#endif
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
            ChestEff.SetActive(false);
            Chest.gameObject.SetActive(true);
            ShowReward(0, 0);

            yield return new WaitForSeconds(0.5f);
            RewardGroup.SetActive(true);

            if (GUIManager.Instance.SoundEnable)
            {
                sound.Play();
            }

            var itemName = rewardsItems[0];

            var rarity = ConfigManager.GetItemDefaultRarity(itemName);

            if( ItemEff[(int)(rarity)] !=null)
            {
                ItemEff[(int)(rarity)].SetActive(true);
            }


            ChestEff.SetActive(true);

            Chest.SetTrigger("open");


            yield return new WaitForSeconds(1f);
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

            GemAvatar.gameObject.SetActive(itemName == CardReward.GEM_CARD);
            GoldAvatar.gameObject.SetActive(itemName == CardReward.GOLD_CARD);

            mCurQuantityNum = 0;

            if (itemName == CardReward.GEM_CARD)
            {
                matAva.gameObject.SetActive(false);
                avatar.gameObject.SetActive(false);
                lbRarity.text = string.Format(Localization.Get("QuantityLabel"), quantity);
            }
            else if (itemName == CardReward.GOLD_CARD)
            {
                matAva.gameObject.SetActive(false);
                avatar.gameObject.SetActive(false);
                lbRarity.text = string.Format(Localization.Get("QuantityLabel"), quantity);
            }
            else if (ConfigManager.Materials.ContainsKey(itemName))
            {
                mCurQuantityNum = 0;
                matAva.gameObject.SetActive(true);
                avatar.gameObject.SetActive(false);
                matAva.SetItem(itemName, quantity);

                lbRarity.text = string.Format(Localization.Get("QuantityLabel"), quantity);
            }
            else
            {
                mCurQuantityNum = quantity;
                matAva.gameObject.SetActive(false);
                avatar.gameObject.SetActive(true);
                var rarity = ConfigManager.GetItemDefaultRarity(itemName);
                int level = 1;
                avatar.SetItem(itemName, rarity, level);
                lbRarity.text = Localization.Get(string.Format("Rariry_" + rarity.ToString()));
            }

            lbItemName.text = ConfigManager.GetDisplayPhanThuong(itemName);
            lbDesc.text = ConfigManager.GetMoTaPhanThuong(itemName);

            //transform.localScale = new Vector3(0, 1, 1);
            //var tween = TweenScale.Begin(gameObject, 0.15f, Vector3.one);
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
        }
    }
}