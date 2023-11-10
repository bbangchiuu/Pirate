using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Drag and Drop item.
/// </summary>
public class MergingItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public static bool dragDisabled = false;										// Drag start global disable

	public static MergingItem draggedItem;                                      // Item that is dragged now
	//public static GameObject icon;                                                  // Icon of dragged item
	public static MergingCell sourceCell;                                       // From this cell dragged item is

	public delegate void DragEvent(MergingItem item);
	public static event DragEvent OnItemDragStartEvent;                             // Drag start event
	public static event DragEvent OnItemDragEndEvent;                               // Drag end event

	private static Canvas canvas;                                                   // Canvas for item drag operation
	private static string canvasName = "DragAndDropCanvas";                   		// Name of canvas
	private static int canvasSortOrder = 100;                                       // Sort order for canvas

	public int Level = 1;
	public string ItemName = "Item1";

	public Transform DisableVisual;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		if (canvas == null)
		{
			GameObject canvasObj = new GameObject(canvasName);
			canvas = canvasObj.AddComponent<Canvas>();
            var rootCanvas = GetComponentInParent<Canvas>();
            canvas.renderMode = rootCanvas.renderMode;
            canvas.worldCamera = rootCanvas.worldCamera;
			canvas.sortingOrder = canvasSortOrder;
            canvasObj.layer = gameObject.layer;
		}
	}

	/// <summary>
	/// This item started to drag.
	/// </summary>
	/// <param name="eventData"></param>
	public void OnBeginDrag(PointerEventData eventData)
	{
		if (dragDisabled == false)
		{
			sourceCell = GetCell();                       							// Remember source cell
			draggedItem = this;                                             		// Set as dragged item
			//// Create item's icon
			//icon = new GameObject();
			//icon.transform.SetParent(canvas.transform);
			//icon.name = "Icon";
			Image myImage = GetComponent<Image>();
			myImage.raycastTarget = false;                                          // Disable icon's raycast for correct drop handling
            //Image iconImage = icon.AddComponent<Image>();
            //iconImage.raycastTarget = false;
            //iconImage.sprite = myImage.sprite;

			// Set icon's dimensions
			RectTransform myRect = GetComponent<RectTransform>();
            myRect.pivot = new Vector2(0.5f, 0.5f);
            myRect.anchorMin = new Vector2(0.5f, 0.5f);
            myRect.anchorMax = new Vector2(0.5f, 0.5f);

            transform.SetParent(canvas.transform);
			//RectTransform iconRect = myImage.GetComponent<RectTransform>();
			
			//iconRect.sizeDelta = new Vector2(myRect.rect.width, myRect.rect.height);

			if (OnItemDragStartEvent != null)
			{
				OnItemDragStartEvent(this);                                			// Notify all items about drag start for raycast disabling
			}
		}
	}

	/// <summary>
	/// Every frame on this item drag.
	/// </summary>
	/// <param name="data"></param>
	public void OnDrag(PointerEventData data)
	{
		if (draggedItem != null)
		{
            Vector3 worldPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(), Input.mousePosition, canvas.worldCamera, out worldPos);
            draggedItem.transform.position = worldPos;                          // Item's icon follows to cursor in screen pixels
		}
	}

	/// <summary>
	/// This item is dropped.
	/// </summary>
	/// <param name="eventData"></param>
	public void OnEndDrag(PointerEventData eventData)
	{
		ResetConditions();
	}

	/// <summary>
	/// Resets all temporary conditions.
	/// </summary>
	private void ResetConditions()
	{
        //if (icon != null)
        //{
        //	Destroy(icon);                                                          // Destroy icon on item drop
        //}
        if (draggedItem)
        {
            if (draggedItem.GetComponentInParent<MergingCell>() == null)   // If item have no cell after drop
            {
                if (sourceCell != null)
                {
                    draggedItem.transform.SetParent(sourceCell.transform);
                }

                draggedItem.transform.localPosition = Vector3.zero;
            }
            
        }
        
        if (OnItemDragEndEvent != null)
		{
			OnItemDragEndEvent(this);                                       		// Notify all cells about item drag end
		}
		draggedItem = null;
		//icon = null;
		sourceCell = null;
	}

	/// <summary>
	/// Enable item's raycast.
	/// </summary>
	/// <param name="condition"> true - enable, false - disable </param>
	public void MakeRaycast(bool condition)
	{
		Image image = GetComponent<Image>();
		if (image != null)
		{
			image.raycastTarget = condition;
		}
	}

	/// <summary>
	/// Gets DaD cell which contains this item.
	/// </summary>
	/// <returns>The cell.</returns>
	public MergingCell GetCell()
	{
		return GetComponentInParent<MergingCell>();
	}

	/// <summary>
	/// Raises the disable event.
	/// </summary>
	void OnDisable()
	{
		ResetConditions();
	}

	public void UpgradeItem()
    {
		Level++;
		gameObject.name = ItemName + Level;
	}
}
