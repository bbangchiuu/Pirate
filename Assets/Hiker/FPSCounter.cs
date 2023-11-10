using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using NGUI;

//[Beebyte.Obfuscator.Skip]
public class FPSCounter : MonoBehaviour
{
    public static FPSCounter instance;
    public Text displayText;

    // Use this for initialization
    float time;
    int count;

    public int FPS { get; private set; }

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    void Start()
    {
        //#if TEST_BUILD && !VIDEO_BUILD
        //        this.gameObject.SetActive(true);
        //#else
        //        this.gameObject.SetActive(false);
        //#endif
        //        // time = Time.time;
        count = 0;
        time = 0;
    }

    void UpdateFPS()
    {
        time += Time.unscaledDeltaTime;
        count++;

        if (time >= 3f && displayText != null)
        {
            int fps = Mathf.FloorToInt(count / time);
            displayText.text = fps.ToString();
            FPS = fps;
            count = 0;
            time = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFPS();
    }
}
