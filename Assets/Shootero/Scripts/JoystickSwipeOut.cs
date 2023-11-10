using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Joystick))]
public class JoystickSwipeOut : MonoBehaviour
{
    Joystick joystick;
    public float MaxTimeActive = 0.2f;
    public float MinValueActive = 1.3f;

    public event System.Action<Vector2> onActivate;
    
    float timeDown = 0;
    private void Awake()
    {
        joystick = GetComponent<Joystick>();
    }

    private void OnEnable()
    {
        joystick.onPointerDownEvent += OnPointerDownEvent;
        joystick.onPointerUpEvent += OnPointerUpEvent;
    }

    private void OnDisable()
    {
        joystick.onPointerDownEvent -= OnPointerDownEvent;
        joystick.onPointerUpEvent -= OnPointerUpEvent;
    }

    private void Update()
    {
        timeDown += Time.unscaledDeltaTime;
    }

    void OnPointerDownEvent(PointerEventData eventData)
    {
        timeDown = 0;
    }

    void OnPointerUpEvent(PointerEventData eventData)
    {
        if (timeDown <= MaxTimeActive)
        {
            if (joystick.RawInput.magnitude >= MinValueActive)
            {
                onActivate?.Invoke(joystick.RawInput);
            }
        }
    }
}
