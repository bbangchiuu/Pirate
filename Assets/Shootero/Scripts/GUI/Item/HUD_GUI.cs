using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD_GUI : MonoBehaviour
{
    public Text label;
    public TextMeshProUGUI textPro;
    public float speedUp = 10f;
    float displayTime;

    public void Display(string t, float time, Color color)
    {
        gameObject.SetActive(true);

        if (label)
        {
            label.text = t;
            displayTime = time;
            label.transform.localPosition = Vector3.zero;
            label.color = color;

            StopAllCoroutines();

            var labelAlpha = label.GetComponent<TweenAlpha>();
            if (labelAlpha)
            {
                labelAlpha.ignoreTimeScale = true;
                labelAlpha.duration = time;
                labelAlpha.ResetToBeginning();
                labelAlpha.enabled = true;
            }

            var labelScale = label.GetComponent<TweenScale>();
            if (labelScale)
            {
                labelScale.ignoreTimeScale = true;
                labelScale.duration = time;
                labelScale.ResetToBeginning();
                labelScale.enabled = true;
            }
        }
        if (textPro)
        {
            textPro.text = t;
            displayTime = time;
            textPro.transform.localPosition = Vector3.zero;
            //textPro.color = color;

            StopAllCoroutines();

            var labelAlpha = textPro.GetComponent<TweenAlpha>();
            if (labelAlpha)
            {
                labelAlpha.ignoreTimeScale = true;
                labelAlpha.duration = time;
                labelAlpha.ResetToBeginning();
                labelAlpha.enabled = true;
            }

            var labelScale = textPro.GetComponent<TweenScale>();
            if (labelScale)
            {
                labelScale.ignoreTimeScale = true;
                labelScale.duration = time;
                labelScale.ResetToBeginning();
                labelScale.enabled = true;
            }
        }
    }

    private void Update()
    {
        displayTime -= Time.deltaTime;

        if (displayTime > 0)
        {
            label.transform.Translate(Vector3.up * speedUp * Time.unscaledDeltaTime, Space.Self);
        }
        else
        {
            //gameObject.SetActive(false);
            ObjectPoolManager.Unspawn(gameObject);
        }
    }
}
