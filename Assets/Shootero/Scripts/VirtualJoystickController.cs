using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

[RequireComponent(typeof(Joystick))]
public class VirtualJoystickController : MonoBehaviour
{
    public string horizontalAxisName = "Horizontal";// The name given to the horizontal axis for the cross platform input
    public string verticalAxisName = "Vertical";    // The name given to the vertical axis for the cross platform input 

    bool useX;
    bool useY;
    private CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis;               // Reference to the joystick in the cross platform input
    private CrossPlatformInputManager.VirtualAxis verticalVirtualAxis;                 // Reference to the joystick in the cross platform input

    Joystick joystick;
    public Joystick GetJoyStick() { if (joystick == null) joystick = GetComponent<Joystick>(); return joystick; }

    private void UpdateVirtualAxes()
    {
        var j = GetJoyStick();
        if (useX)
            horizontalVirtualAxis.Update(j.Horizontal);

        if (useY)
            verticalVirtualAxis.Update(j.Vertical);

    }

    private void CreateVirtualAxes()
    {
        var j = GetJoyStick();
        // set axes to use
        useX = (j.AxisOptions == AxisOptions.Both || j.AxisOptions == AxisOptions.Horizontal);
        useY = (j.AxisOptions == AxisOptions.Both || j.AxisOptions == AxisOptions.Vertical);

        // create new axes based on axes to use
        if (useX)
            horizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
        if (useY)
            verticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
    }

    private void Update()
    {
        UpdateVirtualAxes();
    }

    private void FixedUpdate()
    {
        UpdateVirtualAxes();
    }

    void OnEnable()
    {
        CreateVirtualAxes();
    }

    void OnDisable()
    {
        // remove the joysticks from the cross platform input
        if (useX)
        {
            horizontalVirtualAxis.Remove();
        }
        if (useY)
        {
            verticalVirtualAxis.Remove();
        }
    }
}
