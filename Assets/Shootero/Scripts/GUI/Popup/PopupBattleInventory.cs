using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    public class PopupBattleInventory : PopupBase
    {
        public static PopupBattleInventory instance;

        private BattleInventory battleInvent;

        [SerializeField] private Button closeBtn;
        [Header("Slot")]
        [SerializeField] private GameObject slotPrefabs;
        [SerializeField] private Transform slotContainer;
        private Dictionary<string, SlotGuiItem> slotDic;
        [Header("Hand")]
        [SerializeField] private GameObject handSlotPrefabs;
        [SerializeField] private Transform handContainer;
        private Dictionary<string, SlotGuiItem> handDic;
        [Header("Random")]
        [SerializeField] private GameObject randomSlotPrefabs;
        [SerializeField] private Transform randomContainer;
        private Dictionary<string, SlotGuiItem> randomDic;

        public int randomCount = 3;
        public List<BattleItem> randomItem = new List<BattleItem>();
        public Dictionary<string, SlotInventory> randomSlotInventory;

        [Header("FollowItem")]
        [SerializeField] private GameObject followItem;

        private PointerEventData m_PointerEventData;
        private GraphicRaycaster graphicRaycasterPopupContainer;
        private EventSystem m_EventSystem;
        private Camera camera;

        private void Start()
        {
            graphicRaycasterPopupContainer = GUIManager.Instance.graphicRaycasterPopupContainer;
            m_EventSystem = EventSystem.current;
            camera = GUIManager.Instance.screenSpaceCamera;
            closeBtn.onClick.AddListener(OnCloseBtnClick);
        }

        public static PopupBattleInventory Create(BattleInventory battleInventory, bool randomWeapon = true)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupBattleInventory");
            instance = go.GetComponent<PopupBattleInventory>();
            instance.Init(battleInventory, randomWeapon);
            return instance;
        }

        private void Init(BattleInventory battleInventory, bool randomWeapon)
        {
            slotDic = new Dictionary<string, SlotGuiItem>();
            handDic = new Dictionary<string, SlotGuiItem>();
            randomDic = new Dictionary<string, SlotGuiItem>();

            battleInvent = battleInventory;

            foreach(var key in battleInvent.slotDic.Keys)
            {
                var sl = Instantiate(slotPrefabs, slotContainer);
                sl.gameObject.SetActive(true);
                var slot = sl.GetComponent<SlotGuiItem>();
                slot.Init(battleInvent.slotDic[key]);
                slotDic.Add(key, slot);
            }

            foreach (var key in battleInvent.handDic.Keys)
            {
                var sl = Instantiate(handSlotPrefabs, handContainer);
                sl.gameObject.SetActive(true);
                var slot = sl.GetComponent<SlotGuiItem>();
                slot.Init(battleInvent.handDic[key]);
                handDic.Add(key, slot);
            }

            if (randomWeapon)
            {
                ShowRandomItem();
            }

            LoadItemInInventory();
            ReloadBuffInInventory();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (QuanlyNguoichoi.Instance != null && QuanlyNguoichoi.Instance.PlayerUnit != null)
            {
                QuanlyNguoichoi.Instance.PlayerUnit.UpdateShooterStat();
            }
        }

        public void ShowItemInSlot(string key, BattleItem item)
        {
            slotDic[key].SetItem(item);
        }

        public void ShowItemInHandSlot(string key, BattleItem item)
        {
            handDic[key].SetItem(item);
        }

        public void LoadItemInInventory()
        {
            foreach (var key in handDic.Keys)
            {
                if (battleInvent.itemDic.ContainsKey(key))
                {
                    ShowItemInHandSlot(key, battleInvent.itemDic[key]);
                }
                else
                {
                    ShowItemInHandSlot(key, null);
                }
            }

            foreach (var key in slotDic.Keys)
            {
                if (battleInvent.itemDic.ContainsKey(key))
                {
                    ShowItemInSlot(key, battleInvent.itemDic[key]);
                }
                else
                {
                    ShowItemInSlot(key, null);
                }
            }
        }

        public void ReloadBuffInInventory()
        {
            foreach (var key in slotDic.Keys)
            {
                slotDic[key].SetBuff();
            }
        }

        private void ShowRandomItem()
        {
            randomItem = new List<BattleItem>();
            randomSlotInventory = new Dictionary<string, SlotInventory>();

            for (int i = 0; i < randomCount; ++i)
            {
                BattleItem item = new BattleItem();
                item.posId = string.Format("{0}:{1}", "Random", i);
                item.codeName = ConfigManager.BattleItemCfg.RandomCodeName(ConfigManager.BattleItemStats);
                item.tier = 0;
                item.GetStat();
                randomItem.Add(item);
            }

            for (int i = 0; i < randomCount; ++i)
            {
                SlotInventory slot = new SlotInventory();
                randomSlotInventory.Add(slot.InitRandomSlot(i), slot);
            }

            int itemCount = 0;
            foreach (var key in randomSlotInventory.Keys)
            {
                var sl = Instantiate(randomSlotPrefabs, randomContainer);
                sl.gameObject.SetActive(true);
                var slot = sl.GetComponent<SlotGuiItem>();
                slot.Init(randomSlotInventory[key]);
                randomDic.Add(key, slot);

                randomDic[key].SetItem(randomItem[itemCount]);
                itemCount++;
            }
        }

        public void DisableRandomContainer()
        {
            randomContainer.gameObject.SetActive(false);
        }

        //========================Drag And Drop============================
        //Update drag and drop
        bool isHoldingItem = false;
        double buttonDownStartTime = 0f;
        private SlotGuiItem choosingSlot;
        private SlotGuiItem dropSlot;

        public void Update()
        {
            if(isHoldingItem == false)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    buttonDownStartTime = Time.timeAsDouble;
                    choosingSlot = SlotAtMousePos();
                }
                if (choosingSlot != null && choosingSlot.item != null)
                {
                    if (Time.timeAsDouble - buttonDownStartTime > 0.1f)
                    {
                        followItem.transform.position = MousePosToCanvasPos();
                        followItem.gameObject.SetActive(true);
                        isHoldingItem = true;
                    }
                }
            }

            if (isHoldingItem && choosingSlot != null)
            {
                followItem.transform.position = MousePosToCanvasPos();
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (isHoldingItem)
                {
                    if(choosingSlot.item != null)
                    {
                        //Check drop
                        dropSlot = SlotAtMousePos();
                        if (dropSlot != null && dropSlot.slot.posId != choosingSlot.slot.posId && dropSlot.slot.posId.StartsWith("Random") == false)
                        {
                            if(choosingSlot.slot.posId.StartsWith("Random"))
                            {
                                QuanLyBattleItem.instance.AddBattleItem(choosingSlot.item, dropSlot.slot.posId);
                            }
                            else
                            {
                                battleInvent.SetItemInPosition(dropSlot.slot.posId, choosingSlot.item, true);
                            }
                        }
                    }
                }
                isHoldingItem = false;
                choosingSlot = null;
                dropSlot = null;
                followItem.gameObject.SetActive (false);
            }
        }

        private Vector3 MousePosToCanvasPos()
        {
            Vector3 screenPoint = Input.mousePosition;
            screenPoint.z = graphicRaycasterPopupContainer.transform.position.z;
            return camera.ScreenToWorldPoint(screenPoint);
        }

        private SlotGuiItem SlotAtMousePos()
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = Hiker.Util.ListPool<RaycastResult>.Claim();
            graphicRaycasterPopupContainer.Raycast(m_PointerEventData, results);
            for (int i = 0; i < results.Count; ++i)
            {
                var result = results[i];
                if (result.gameObject && result.gameObject.layer == LayerMan.itemLayer)
                {
                    var item = result.gameObject.GetComponent<SlotGuiItem>();
                    if (item != null)
                    {
                        return item;
                    }
                }
            }
            Hiker.Util.ListPool<RaycastResult>.Release(results);
            return null;
        }


    }
}

