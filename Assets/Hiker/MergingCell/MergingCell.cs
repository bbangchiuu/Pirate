using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Every item's cell must contain this script
/// </summary>
[RequireComponent(typeof(Image))]
public class MergingCell : MonoBehaviour, IDropHandler
{
    public enum CellType                                                    // Cell types
    {
        Swap,                                                               // Items will be swapped between any cells
        DropOnly,                                                           // Item will be dropped into cell
        DragOnly,                                                            // Item will be dragged from this cell
        Merge,                                                              // duongrs merge items to higher lvl
        RandomMerge,                                                        // duongrs merge items to random higher lvl item
    }

    public enum TriggerType                                                 // Types of drag and drop events
    {
        DropRequest,                                                        // Request for item drop from one cell to another
        DropEventEnd,                                                       // Drop event completed
        ItemAdded,                                                          // Item manualy added into cell
        ItemWillBeDestroyed,                                                 // Called just before item will be destroyed
        ItemMerged,
    }

    public class DropEventDescriptor                                        // Info about item's drop event
    {
        public TriggerType triggerType;                                     // Type of drag and drop trigger
        public MergingCell sourceCell;                                  // From this cell item was dragged
        public MergingCell destinationCell;                             // Into this cell item was dropped
        public MergingItem item;                                        // Dropped item
        public bool permission;                                             // Decision need to be made on request
    }

	[Tooltip("Functional type of this cell")]
    public CellType cellType = CellType.Swap;                               // Special type of this cell
	[Tooltip("Sprite color for empty cell")]
    public Color empty = new Color();                                       // Sprite color for empty cell
	[Tooltip("Sprite color for filled cell")]
    public Color full = new Color();                                        // Sprite color for filled cell
	[Tooltip("This cell has unlimited amount of items")]
    public bool unlimitedSource = false;                                    // Item from this cell will be cloned on drag start

	private MergingItem myDadItem;										// Item of this DaD cell

    public Transform DisableVisual;
    public Transform HightlighVisual;

    public void ShowHightlight()
    {
        if (HightlighVisual)
        {
            HightlighVisual.gameObject.SetActive(true);
            HightlighVisual.SetAsLastSibling();
        }
    }

    public void HideHightlight()
    {
        if (HightlighVisual)
        {
            HightlighVisual.gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        MergingItem.OnItemDragStartEvent += OnAnyItemDragStart;         // Handle any item drag start
        MergingItem.OnItemDragEndEvent += OnAnyItemDragEnd;             // Handle any item drag end
		UpdateMyItem();
		UpdateBackgroundState();
    }

    void OnDisable()
    {
        MergingItem.OnItemDragStartEvent -= OnAnyItemDragStart;
        MergingItem.OnItemDragEndEvent -= OnAnyItemDragEnd;
        StopAllCoroutines();                                                // Stop all coroutines if there is any
    }

    /// <summary>
    /// On any item drag start need to disable all items raycast for correct drop operation
    /// </summary>
    /// <param name="item"> dragged item </param>
    private void OnAnyItemDragStart(MergingItem item)
    {
		UpdateMyItem();
		if (myDadItem != null)
        {
			myDadItem.MakeRaycast(false);                                  	// Disable item's raycast for correct drop handling
			if (myDadItem == item)                                         	// If item dragged from this cell
            {
                // Check cell's type
                switch (cellType)
                {
                    case CellType.DropOnly:
                        //MergingItem.icon.SetActive(false);              // Item can not be dragged. Hide icon
                        break;
                }
                if (DisableVisual)
                    DisableVisual.gameObject.SetActive(false);
                if (myDadItem.DisableVisual)
                    myDadItem.DisableVisual.gameObject.SetActive(false);
            }
            else
            {
                bool canMerge = (item.Level < ConfigManager.GiangSinhCfg.MaxLevel) &&
                        (myDadItem.gameObject.name == item.gameObject.name);
                if (DisableVisual)
                    DisableVisual.gameObject.SetActive(canMerge == false);
                if (myDadItem.DisableVisual)
                    myDadItem.DisableVisual.gameObject.SetActive(canMerge == false);
            }
        }
        else
        {
            if (DisableVisual)
                DisableVisual.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// On any item drag end enable all items raycast
    /// </summary>
    /// <param name="item"> dragged item </param>
    private void OnAnyItemDragEnd(MergingItem item)
    {
		UpdateMyItem();
		if (myDadItem != null)
        {
			myDadItem.MakeRaycast(true);                                  	// Enable item's raycast
        }
		UpdateBackgroundState();

        if (DisableVisual)
            DisableVisual.gameObject.SetActive(false);
        if (myDadItem && myDadItem.DisableVisual)
            myDadItem.DisableVisual.gameObject.SetActive(false);
    }

    /// <summary>
    /// Item is dropped in this cell
    /// </summary>
    /// <param name="data"></param>
    public void OnDrop(PointerEventData data)
    {
        if (MergingItem.draggedItem != null)
        {
            MergingItem item = MergingItem.draggedItem;
            MergingCell sourceCell = MergingItem.sourceCell;
            //if (MergingItem.icon.activeSelf == true)                    // If icon inactive do not need to drop item into cell
            {
                if ((item != null) && (sourceCell != this))
                {
                    DropEventDescriptor desc = new DropEventDescriptor();
                    switch (cellType)                                       // Check this cell's type
                    {
                        case CellType.Swap:                                 // Item in destination cell can be swapped
							UpdateMyItem();
                            switch (sourceCell.cellType)
                            {
                                case CellType.Swap:                         // Item in source cell can be swapped
                                    // Fill event descriptor
                                    desc.item = item;
                                    desc.sourceCell = sourceCell;
                                    desc.destinationCell = this;
                                    SendRequest(desc);                      // Send drop request
                                    if (desc.permission && myDadItem != null)
                                    {
                                        var newDesc = new DropEventDescriptor();
                                        newDesc.item = myDadItem;
                                        newDesc.sourceCell = this;
                                        newDesc.destinationCell = sourceCell;
                                        SendRequest(newDesc);
                                        if (newDesc.permission == false)
                                        {
                                            desc.permission = false;
                                        }
                                    }
                                    
                                    StartCoroutine(NotifyOnDragEnd(desc));  // Send notification after drop will be finished
                                    if (desc.permission == true)            // If drop permitted by application
                                    {
										if (desc.destinationCell.myDadItem != null)            // If destination cell has item
                                        {
                                            // Fill event descriptor
                                            DropEventDescriptor descAutoswap = new DropEventDescriptor();
											descAutoswap.item = desc.destinationCell.myDadItem;
                                            descAutoswap.sourceCell = this;
                                            descAutoswap.destinationCell = sourceCell;
                                            SendRequest(descAutoswap);                      // Send drop request
                                            StartCoroutine(NotifyOnDragEnd(descAutoswap));  // Send notification after drop will be finished
                                            if (descAutoswap.permission == true)            // If drop permitted by application
                                            {
                                                SwapItems(desc.sourceCell, desc.destinationCell);                // Swap items between cells
                                            }
                                            else
                                            {
                                                desc.destinationCell.PlaceItem(item);            // Delete old item and place dropped item into this cell
                                            }
                                        }
                                        else
                                        {
                                            desc.destinationCell.PlaceItem(item);                // Place dropped item into this empty cell
                                        }
                                    }
                                    break;
                                default:                                    // Item in source cell can not be swapped
                                    // Fill event descriptor
                                    desc.item = item;
                                    desc.sourceCell = sourceCell;
                                    desc.destinationCell = this;
                                    SendRequest(desc);                      // Send drop request
                                    StartCoroutine(NotifyOnDragEnd(desc));  // Send notification after drop will be finished
                                    if (desc.permission == true)            // If drop permitted by application
                                    {
										desc.destinationCell.PlaceItem(item);                    // Place dropped item into this cell
                                    }
                                    break;
                            }
                            break;
                        case CellType.DropOnly:                             // Item only can be dropped into destination cell
                            // Fill event descriptor
                            desc.item = item;
                            desc.sourceCell = sourceCell;
                            desc.destinationCell = this;
                            SendRequest(desc);                              // Send drop request
                            StartCoroutine(NotifyOnDragEnd(desc));          // Send notification after drop will be finished
                            if (desc.permission == true)                    // If drop permitted by application
                            {
                                desc.destinationCell.PlaceItem(item);                            // Place dropped item in this cell
                            }
                            break;
                        case CellType.Merge:                             // Item only can be dropped into destination cell
                            // Fill event descriptor
                            if (myDadItem != null && item != null)
                            {
                                desc.item = item;
                                desc.sourceCell = sourceCell;
                                desc.destinationCell = this;
                                SendRequest(desc);                              // Send drop request
                                StartCoroutine(NotifyOnDragEnd(desc));          // Send notification after drop will be finished
                                desc.triggerType = TriggerType.ItemMerged;
                                SendNotification(desc);
                                if (desc.permission == true)                    // If drop permitted by application
                                {
                                    //desc.destinationCell.PlaceItem(item);                            // Place dropped item in this cell
                                    //sourceCell.RemoveItem();
                                    Destroy(item.gameObject);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
			UpdateMyItem();
			UpdateBackgroundState();
			sourceCell.UpdateMyItem();
			sourceCell.UpdateBackgroundState();
        }
    }

	/// <summary>
	/// Put item into this cell.
	/// </summary>
	/// <param name="item">Item.</param>
	private void PlaceItem(MergingItem item)
	{
		if (item != null)
		{
			DestroyItem();                                            	// Remove current item from this cell
			myDadItem = null;
			MergingCell cell = item.GetComponentInParent<MergingCell>();
			if (cell != null)
			{
				if (cell.unlimitedSource == true)
				{
					string itemName = item.name;
					item = Instantiate(item);                               // Clone item from source cell
					item.name = itemName;
				}
			}
			item.transform.SetParent(transform, false);
			item.transform.localPosition = Vector3.zero;
			item.MakeRaycast(true);
			myDadItem = item;
		}
		UpdateBackgroundState();
	}

    /// <summary>
    /// Destroy item in this cell
    /// </summary>
    private void DestroyItem()
    {
		UpdateMyItem();
		if (myDadItem != null)
        {
            DropEventDescriptor desc = new DropEventDescriptor();
            // Fill event descriptor
            desc.triggerType = TriggerType.ItemWillBeDestroyed;
			desc.item = myDadItem;
            desc.sourceCell = this;
            desc.destinationCell = this;
            SendNotification(desc);                                         // Notify application about item destruction
			if (myDadItem != null)
			{
				Destroy(myDadItem.gameObject);
			}
        }
		myDadItem = null;
		UpdateBackgroundState();
    }

    /// <summary>
    /// Send drag and drop information to application
    /// </summary>
    /// <param name="desc"> drag and drop event descriptor </param>
    private void SendNotification(DropEventDescriptor desc)
    {
        if (desc != null)
        {
            // Send message with DragAndDrop info to parents GameObjects
            gameObject.SendMessageUpwards("OnSimpleDragAndDropEvent", desc, SendMessageOptions.DontRequireReceiver);
        }
    }

    /// <summary>
    /// Send drag and drop request to application
    /// </summary>
    /// <param name="desc"> drag and drop event descriptor </param>
    /// <returns> result from desc.permission </returns>
    private bool SendRequest(DropEventDescriptor desc)
    {
        bool result = false;
        if (desc != null)
        {
            desc.triggerType = TriggerType.DropRequest;
            desc.permission = true;
            SendNotification(desc);
            result = desc.permission;
        }
        return result;
    }

    /// <summary>
    /// Wait for event end and send notification to application
    /// </summary>
    /// <param name="desc"> drag and drop event descriptor </param>
    /// <returns></returns>
    private IEnumerator NotifyOnDragEnd(DropEventDescriptor desc)
    {
        // Wait end of drag operation
        while (MergingItem.draggedItem != null)
        {
            yield return new WaitForEndOfFrame();
        }
        desc.triggerType = TriggerType.DropEventEnd;
        SendNotification(desc);
    }

	/// <summary>
	/// Change cell's sprite color on item put/remove.
	/// </summary>
	/// <param name="condition"> true - filled, false - empty </param>
	public void UpdateBackgroundState()
	{
		Image bg = GetComponent<Image>();
		if (bg != null)
		{
			bg.color = myDadItem != null ? full : empty;
		}
	}

	/// <summary>
	/// Updates my item
	/// </summary>
	public void UpdateMyItem()
	{
		myDadItem = GetComponentInChildren<MergingItem>();
	}

	/// <summary>
	/// Get item from this cell
	/// </summary>
	/// <returns> Item </returns>
	public MergingItem GetItem()
	{
		return myDadItem;
	}

    /// <summary>
    /// Manualy add item into this cell
    /// </summary>
    /// <param name="newItem"> New item </param>
    public void AddItem(MergingItem newItem)
    {
        if (newItem != null)
        {
            if (newItem.gameObject.activeSelf == false)
            {
                newItem.gameObject.SetActive(true);
            }
			PlaceItem(newItem);
            DropEventDescriptor desc = new DropEventDescriptor();
            // Fill event descriptor
            desc.triggerType = TriggerType.ItemAdded;
            desc.item = newItem;
            desc.sourceCell = this;
            desc.destinationCell = this;
            SendNotification(desc);
        }
    }

    /// <summary>
    /// Manualy delete item from this cell
    /// </summary>
    public void RemoveItem()
    {
        HideHightlight();
        DestroyItem();
    }

	/// <summary>
	/// Swap items between two cells
	/// </summary>
	/// <param name="firstCell"> Cell </param>
	/// <param name="secondCell"> Cell </param>
	public void SwapItems(MergingCell firstCell, MergingCell secondCell)
	{
		if ((firstCell != null) && (secondCell != null))
		{
			MergingItem firstItem = firstCell.GetItem();                // Get item from first cell
			MergingItem secondItem = secondCell.GetItem();              // Get item from second cell
			// Swap items
			if (firstItem != null)
			{
				firstItem.transform.SetParent(secondCell.transform, false);
				firstItem.transform.localPosition = Vector3.zero;
				firstItem.MakeRaycast(true);
			}
			if (secondItem != null)
			{
				secondItem.transform.SetParent(firstCell.transform, false);
				secondItem.transform.localPosition = Vector3.zero;
				secondItem.MakeRaycast(true);
			}
			// Update states
			firstCell.UpdateMyItem();
			secondCell.UpdateMyItem();
			firstCell.UpdateBackgroundState();
			secondCell.UpdateBackgroundState();
		}
	}
}
