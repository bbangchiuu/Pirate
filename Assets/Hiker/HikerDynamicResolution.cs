using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HikerDynamicResolution : MonoBehaviour
{
    public Text screenText;

    FrameTiming[] frameTimings = new FrameTiming[3];

    public float maxResolutionWidthScale = 1.0f;
    public float maxResolutionHeightScale = 1.0f;
    public float minResolutionWidthScale = 0.5f;
    public float minResolutionHeightScale = 0.5f;
    public float scaleWidthIncrement = 0.1f;
    public float scaleHeightIncrement = 0.1f;

    float m_widthScale = 1.0f;
    float m_heightScale = 1.0f;

    // Variables for dynamic resolution algorithm that persist across frames
    uint m_frameCount = 0;

    const uint kNumFrameTimings = 2;

    double m_gpuFrameTime;
    double m_cpuFrameTime;
    const double targetFrameTime = 1000d / 60d;
    const double frameTimeThresold = targetFrameTime / 10d;
    double highFrameTime = targetFrameTime;
    double lowFrameTime = targetFrameTime - frameTimeThresold;
    public int ResWidth
    {
        get { return (int)Mathf.Ceil(ScalableBufferManager.widthScaleFactor * Screen.currentResolution.width); }
    }
    public int ResHeight
    {
        get { return (int)Mathf.Ceil(ScalableBufferManager.heightScaleFactor * Screen.currentResolution.height); }
    }
    public static HikerDynamicResolution instance = null;
    private void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start()
    {
        //int rezWidth = (int)Mathf.Ceil(ScalableBufferManager.widthScaleFactor * Screen.currentResolution.width);
        //int rezHeight = (int)Mathf.Ceil(ScalableBufferManager.heightScaleFactor * Screen.currentResolution.height);
        //screenText.text = string.Format("Scale: {0:F3}x{1:F3}\nResolution: {2}x{3}\n",
        //    m_widthScale,
        //    m_heightScale,
        //    rezWidth,
        //    rezHeight);
    }

    // Update is called once per frame
    void Update()
    {
        float oldWidthScale = m_widthScale;
        float oldHeightScale = m_heightScale;

        //// One finger lowers the resolution
        //if (Input.GetButtonDown("Fire1"))
        //{
        //    m_heightScale = Mathf.Max(minResolutionHeightScale, m_heightScale - scaleHeightIncrement);
        //    m_widthScale = Mathf.Max(minResolutionWidthScale, m_widthScale - scaleWidthIncrement);
        //}

        //// Two fingers raises the resolution
        //if (Input.GetButtonDown("Fire2"))
        //{
        //    m_heightScale = Mathf.Min(maxResolutionHeightScale, m_heightScale + scaleHeightIncrement);
        //    m_widthScale = Mathf.Min(maxResolutionWidthScale, m_widthScale + scaleWidthIncrement);
        //}

        var frameTime = /*m_gpuFrameTime < m_cpuFrameTime ? m_cpuFrameTime : */m_gpuFrameTime;

        if (frameTime > highFrameTime)
        {
            m_heightScale = Mathf.Max(minResolutionHeightScale, m_heightScale - scaleHeightIncrement);
            m_widthScale = Mathf.Max(minResolutionWidthScale, m_widthScale - scaleWidthIncrement);
        }
        else if (frameTime < lowFrameTime)
        {
            m_heightScale = Mathf.Min(maxResolutionHeightScale, m_heightScale + scaleHeightIncrement);
            m_widthScale = Mathf.Min(maxResolutionWidthScale, m_widthScale + scaleWidthIncrement);
        }

        if (m_widthScale != oldWidthScale || m_heightScale != oldHeightScale)
        {
            ScalableBufferManager.ResizeBuffers(m_widthScale, m_heightScale);
        }
        DetermineResolution();
        //int rezWidth = (int)Mathf.Ceil(ScalableBufferManager.widthScaleFactor * Screen.currentResolution.width);
        //int rezHeight = (int)Mathf.Ceil(ScalableBufferManager.heightScaleFactor * Screen.currentResolution.height);
        //screenText.text = string.Format("Scale: {0:F3}x{1:F3}\nResolution: {2}x{3}\nScaleFactor: {4:F3}x{5:F3}\nGPU: {6:F3} CPU: {7:F3}",
        //    m_widthScale,
        //    m_heightScale,
        //    rezWidth,
        //    rezHeight,
        //    ScalableBufferManager.widthScaleFactor,
        //    ScalableBufferManager.heightScaleFactor,
        //    m_gpuFrameTime,
        //    m_cpuFrameTime);

        if (screenText)
        {
            if (FPSCounter.instance && FPSCounter.instance.gameObject.activeInHierarchy)
            {
                if (screenText.gameObject.activeSelf == false)
                    screenText.gameObject.SetActive(true);

                screenText.text = string.Format("Scale: {0:F3}x{1:F3}\nResolution: {2}x{3}\nScaleFactor: {4:F3}x{5:F3}\nGPU: {6:F3} CPU: {7:F3}",
                    m_widthScale,
                    m_heightScale,
                    ResWidth,
                    ResHeight,
                    ScalableBufferManager.widthScaleFactor,
                    ScalableBufferManager.heightScaleFactor,
                    m_gpuFrameTime,
                    m_cpuFrameTime
                    );
            }
            else
            {
                if (screenText.gameObject.activeSelf)
                    screenText.gameObject.SetActive(false);
            }
        }
    }

    // Estimate the next frame time and update the resolution scale if necessary.
    private void DetermineResolution()
    {
        ++m_frameCount;
        if (m_frameCount <= kNumFrameTimings)
        {
            return;
        }
        FrameTimingManager.CaptureFrameTimings();
        var numFrameData = FrameTimingManager.GetLatestTimings(kNumFrameTimings, frameTimings);
        if (frameTimings.Length < kNumFrameTimings)
        {
#if DEBUG
            Debug.LogFormat("Skipping frame {0}, didn't get enough frame timings.",
                m_frameCount);
#endif
            return;
        }

        m_gpuFrameTime = frameTimings[0].gpuFrameTime;// + frameTimings[1].gpuFrameTime) * 0.5;
        m_cpuFrameTime = frameTimings[0].cpuFrameTime;// + frameTimings[1].cpuFrameTime) * 0.5;
    }
}
