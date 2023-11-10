using DanielLochner.Assets.SimpleScrollSnap;
using Hiker.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(HikerScrollRect))]
public class HikerSimpleScrollSnapConflictManager : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public SimpleScrollSnap ParentScrollRect;
    private HikerScrollRect _myScrollRect;
    //This tracks if the other one should be scrolling instead of the current one.
    private bool scrollOther;
    //This tracks wether the other one should scroll horizontally or vertically.
    private bool scrollOtherHorizontally;

    void Awake()
    {
        //Get the current scroll rect so we can disable it if the other one is scrolling
        _myScrollRect = this.GetComponent<HikerScrollRect>();
        //If the current scroll Rect has the vertical checked then the other one will be scrolling horizontally.
        scrollOtherHorizontally = _myScrollRect.vertical;
        //Check some attributes to let the user know if this wont work as expected
        if (scrollOtherHorizontally)
        {
            if (_myScrollRect.horizontal)
                Debug.Log("You have added the SecondScrollRect to a scroll view that already has both directions selected");
            if (!(ParentScrollRect.movementType == SimpleScrollSnap.MovementType.Fixed &&
                ParentScrollRect.movementAxis == SimpleScrollSnap.MovementAxis.Horizontal))
                Debug.Log("The other scroll rect doesnt support scrolling horizontally");
        }
        else if (!(ParentScrollRect.movementType == SimpleScrollSnap.MovementType.Fixed &&
                ParentScrollRect.movementAxis == SimpleScrollSnap.MovementAxis.Vertical))
        {
            Debug.Log("The other scroll rect doesnt support scrolling vertically");
        }
    }

    //IBeginDragHandler
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Get the absolute values of the x and y differences so we can see which one is bigger and scroll the other scroll rect accordingly
        float horizontal = Mathf.Abs(eventData.position.x - eventData.pressPosition.x);
        float vertical = Mathf.Abs(eventData.position.y - eventData.pressPosition.y);
        if (scrollOtherHorizontally)
        {
            if (horizontal > vertical)
            {
                scrollOther = true;
                //disable the current scroll rect so it doesnt move.
                _myScrollRect.enabled = false;
                ParentScrollRect.OnBeginDrag(eventData);
                ParentScrollRect.ScrollRect.OnBeginDrag(eventData);
            }
        }
        else if (vertical > horizontal)
        {
            scrollOther = true;
            //disable the current scroll rect so it doesnt move.
            _myScrollRect.enabled = false;
            ParentScrollRect.OnBeginDrag(eventData);
            ParentScrollRect.ScrollRect.OnBeginDrag(eventData);
        }
    }

    //IEndDragHandler
    public void OnEndDrag(PointerEventData eventData)
    {
        if (scrollOther)
        {
            scrollOther = false;
            _myScrollRect.enabled = true;
            ParentScrollRect.OnEndDrag(eventData);
            ParentScrollRect.ScrollRect.OnEndDrag(eventData);
        }
    }

    //IDragHandler
    public void OnDrag(PointerEventData eventData)
    {
        if (scrollOther)
        {
            ParentScrollRect.OnDrag(eventData);
            ParentScrollRect.ScrollRect.OnDrag(eventData);
        }
    }
}
