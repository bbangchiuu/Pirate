using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HikerJoystickIndicator : MonoBehaviour
{
    public bool autoHide = true;
    public Image visualIndicator;
    public Joystick joystick;

    private void Start()
    {
        visualIndicator.CrossFadeAlpha(0, 0.05f, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (joystick.Vertical != 0 || joystick.Horizontal != 0)
        {
            visualIndicator.CrossFadeAlpha(1, 0.1f, true);
            transform.up = new Vector2(joystick.Horizontal, joystick.Vertical);
        }
        else
        {
            visualIndicator.CrossFadeAlpha(0, 0.1f, true);
        }
    }
}
