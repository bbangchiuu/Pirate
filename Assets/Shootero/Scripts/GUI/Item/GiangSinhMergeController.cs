using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Hiker.GUI;

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

/// <summary>
/// Example of control application for drag and drop events handle
/// </summary>
public class GiangSinhMergeController : MonoBehaviour
{
    public MergingCell[] Tables;
    public MergingItem sampleItem;
    public SpriteCollection itemsCollection;

    public int Act { get; set; }
    public int RandomSeed { get; set; }
    public Random.State RandomState { get; set; }

    public string[] TableData { get; set; }

    //GiangSinhGamerData giangSinhData;

    float timeFreeze = 0;

    public AudioClip mergeSound;

    public int GetRandomNumber(int max)
    {
        var curState = Random.state;
        Random.InitState(RandomSeed);
        Random.state = RandomState;
        var result = Random.Range(0, max);
        RandomState = Random.state;
        Random.state = curState;
        return result;
    }

    public bool IsFull()
    {
        foreach (string cell in TableData)
        {
            if (string.IsNullOrEmpty(cell))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Operate all drag and drop requests and events from children cells
    /// </summary>
    /// <param name="desc"> request or event descriptor </param>
    void OnSimpleDragAndDropEvent(MergingCell.DropEventDescriptor desc)
    {
        if (timeFreeze > 0)
        {
            desc.permission = false;
            return;
        }
        // Get control unit of source cell
        GiangSinhMergeController sourceSheet = desc.sourceCell.GetComponentInParent<GiangSinhMergeController>();
        // Get control unit of destination cell
        GiangSinhMergeController destinationSheet = desc.destinationCell.GetComponentInParent<GiangSinhMergeController>();
        switch (desc.triggerType)                                               // What type event is?
        {
            case MergingCell.TriggerType.DropRequest:                       // Request for item drag (note: do not destroy item on request)
                //Debug.Log("Request " + desc.item.name + " from " + sourceSheet.name + " to " + destinationSheet.name);
                break;
            case MergingCell.TriggerType.DropEventEnd:                      // Drop event completed (successful or not)
                //if (desc.permission == true)                                    // If drop successful (was permitted before)
                //{
                //    Debug.Log("Successful drop " + desc.item.name + " from " + sourceSheet.name + " to " + destinationSheet.name);
                //}
                //else                                                            // If drop unsuccessful (was denied before)
                //{
                //    Debug.Log("Denied drop " + desc.item.name + " from " + sourceSheet.name + " to " + destinationSheet.name);
                //}
                break;
            case MergingCell.TriggerType.ItemAdded:                         // New item is added from application
                //Debug.Log("Item " + desc.item.name + " added into " + destinationSheet.name);
                if (desc.item.Level >= ConfigManager.GiangSinhCfg.MaxLevel)
                {
                    var cell = desc.destinationCell;
                    Hiker.HikerUtils.DoAction(cell,
                        () => {
                            cell.UpdateMyItem();
                            var item = cell.GetItem();
                            if (item != null && item.Level >= ConfigManager.GiangSinhCfg.MaxLevel)
                            {
                                cell.ShowHightlight();
                            }
                        },
                        0.5f, true);
                }
                break;
            case MergingCell.TriggerType.ItemWillBeDestroyed:               // Called before item be destructed (can not be canceled)
                //Debug.Log("Item " + desc.item.name + " will be destroyed from " + sourceSheet.name);
                break;
            case MergingCell.TriggerType.ItemMerged:
                var sourceItem = desc.item;
                var destItem = desc.destinationCell.GetItem();

                if (sourceItem == null || destItem == null ||
                    sourceItem.gameObject.name != destItem.gameObject.name)
                {
                    desc.permission = false;
                    break;
                }

                if (sourceItem.Level >= ConfigManager.GiangSinhCfg.MaxLevel)
                {
                    desc.permission = false;
                    break;
                }

                var lastLvl = sourceItem.Level;

                desc.sourceCell.RemoveItem();
                desc.destinationCell.RemoveItem();

                var itemIdx = GetRandomNumber(ConfigManager.GiangSinhCfg.Items.Length);
                string itemName = ConfigManager.GiangSinhCfg.Items[itemIdx];
                var newItem = CreateItem(itemName, lastLvl + 1);
                desc.destinationCell.AddItem(newItem);

                if (this.TableData != null)
                {
                    int srcIdx = -1;
                    int desIdx = -1;
                    for (int i = 0; i < Tables.Length; ++i)
                    {
                        if (Tables[i] == desc.sourceCell)
                        {
                            srcIdx = i;
                        }
                        else if (Tables[i] == desc.destinationCell)
                        {
                            desIdx = i;
                        }
                    }
                    this.TableData[srcIdx] = string.Empty;
                    this.TableData[desIdx] = newItem.gameObject.name;

                    Act++;
                    GameClient.instance.RequestGiangSinhMerge(Act,
                        JsonUtility.ToJson(RandomState),
                        srcIdx, desIdx, newItem.gameObject.name, TableData);
                    if (mergeSound != null)
                    {
                        if (Hiker.GUI.GUIManager.Instance.SoundEnable)
                        {
                            NGUITools.PlaySound(mergeSound);
                        }
                    }
                }

                break;
            default:
                //Debug.Log("Unknown drag and drop event");
                break;
        }
    }

    public void AddRandomItem()
    {
        if (timeFreeze > 0)
        {
            return;
        }
        var itemIdx = GetRandomNumber(ConfigManager.GiangSinhCfg.Items.Length);
        string itemName = ConfigManager.GiangSinhCfg.Items[itemIdx];
        var item = CreateItem(itemName, 1);
        AddItemInFreeCell(item);
    }

    /// <summary>
    /// Add item in first free cell
    /// </summary>
    /// <param name="item"> new item </param>
    public void AddItemInFreeCell(MergingItem item)
    {
        var listTemp = Hiker.Util.ListPool<int>.Claim();
        for (int i = 0; i < Tables.Length; ++i)
        {
            Tables[i].UpdateMyItem();
            if (Tables[i].GetItem() == null)
            {
                listTemp.Add(i);
            }
        }

        MergingCell freeCell = null;
        int slot = -1;
        if (listTemp.Count > 0)
        {
            var r = GetRandomNumber(listTemp.Count);
            slot = listTemp[r];
            freeCell = Tables[slot];
        }

        Hiker.Util.ListPool<int>.Release(listTemp);
        if (freeCell != null)
        {
            freeCell.AddItem(item);
            if (TableData != null)
            {
                TableData[slot] = item.gameObject.name;
            }

            Act++;
            GameClient.instance.RequestGiangSinhDraw(Act,
                        JsonUtility.ToJson(RandomState),
                        slot, item.gameObject.name, TableData);
        }
    }

    MergingItem CreateItem(string itemName, int lvl)
    {
        var newItem = Instantiate(sampleItem, transform.GetComponent<RectTransform>());
        newItem.Level = lvl;
        newItem.ItemName = itemName;
        newItem.name = itemName + lvl;
        newItem.GetComponent<Image>().sprite = itemsCollection.GetSprite(newItem.name);

        var btn = newItem.gameObject.AddMissingComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.transition = Selectable.Transition.None;
        btn.onClick.AddListener(delegate { OnItemClick(newItem); });

        return newItem;
    }

    void OnItemClick(MergingItem item)
    {
        var cell = item.GetCell();
        if (cell != null)
        {
            int slot = -1;
            for (int i = 0; i < Tables.Length; ++i)
            {
                if (Tables[i] == cell)
                {
                    slot = i;
                    break;
                }
            }
            var itemName = TableData[slot];
            int lvl = GiangSinhConfig.GetLevelFromName(itemName);
            if (lvl >= ConfigManager.GiangSinhCfg.MaxLevel)
            {
                Act++;
                TableData[slot] = string.Empty;
                cell.RemoveItem();
                GameClient.instance.RequestGiangSinhActiveItem(Act,
                        JsonUtility.ToJson(RandomState),
                        slot, TableData);
            }
        }
    }

    public void Init(GiangSinhGamerData giangSinhData)
    {
        //this.giangSinhData = giangSinhData;
        RandomSeed = giangSinhData.Seed;

        if (string.IsNullOrEmpty(giangSinhData.State))
        {
            var curState = Random.state;
            Random.InitState(giangSinhData.Seed);
            RandomState = Random.state;
            Random.state = curState;
        }
        else
        {
            RandomState = JsonUtility.FromJson<Random.State>(giangSinhData.State);
        }
        Act = giangSinhData.Act;

        SyncWithItems(giangSinhData.Table);
    }

    public void SyncWithItems(string[] tableItems)
    {
        this.TableData = new string[tableItems.Length];
        for (int i = 0; i < tableItems.Length; ++i)
        {
            this.TableData[i] = tableItems[i];
        }

        for (int i = 0; i < Tables.Length; ++i)
        {
            var cell = Tables[i];
            var curItem = cell.GetItem();
            if (i < tableItems.Length)
            {
                var itemNameLvl = tableItems[i];
                if (string.IsNullOrEmpty(itemNameLvl))
                {
                    cell.RemoveItem();
                }
                else
                {
                    if (curItem != null)
                    {
                        if (curItem.gameObject.name != itemNameLvl)
                            cell.RemoveItem();
                    }
                    curItem = cell.GetItem();
                    if (curItem == null)
                    {
                        var itemName = itemNameLvl.Substring(0, itemNameLvl.Length - 1);
                        var lvl = GiangSinhConfig.GetLevelFromName(itemNameLvl);

                        var item = CreateItem(itemName, lvl);
                        if (item != null)
                        {
                            cell.AddItem(item);
                        }
                    }
                }
            }
            else if (curItem != null)
            {
                cell.RemoveItem();
            }
        }
    }

    private void Update()
    {
        if (timeFreeze > 0)
        {
            timeFreeze -= Time.deltaTime;
        }
    }

    public void Freeze(float time, bool force)
    {
        if (force)
        {
            timeFreeze = time;
        }
        else if (timeFreeze < time)
        {
            timeFreeze = time;
        }
    }
}
