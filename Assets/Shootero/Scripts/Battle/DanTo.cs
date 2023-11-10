using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanTo : MonoBehaviour
{
    SatThuongDT mDan;
    bool inited = false;
    public Vector3 originScale { get; private set; }
    public float TileScale { get; private set; }

    private List<DanToSrc> listSrc = new List<DanToSrc>();

    private void Awake()
    {
        mDan = GetComponent<SatThuongDT>();
    }

    protected void Update()
    {
        if (inited)
        {

        }
    }

    public void InitOriginSize(Vector3 scale)
    {
        if (inited == false)
        {
            originScale = scale;
            TileScale = 1f;
            listSrc.Clear();
        }
    }

    public void ApplyScale(DanToSrc src)
    {
        float scale = 1f;
        bool existed = false;
        for (int i = listSrc.Count - 1; i >= 0; --i)
        {
            var item = listSrc[i];
            if (item.Src == src.Src)
            {
                listSrc[i] = src;
                scale *= src.Scale;
                existed = true;
            }
            else
            {
                scale *= item.Scale;
            }
        }
        if (existed == false)
        {
            listSrc.Add(src);
            scale *= src.Scale;
        }

        ScaleTheoTile(scale);
    }

    void ScaleTheoTile(float scale)
    {
        TileScale = scale;
        var parent = transform.parent;
        if (parent != null)
        {
            transform.SetParent(null);
        }
        transform.localScale = originScale * TileScale;
        if (parent)
        {
            transform.SetParent(parent);
        }
    }

    public void ApplyScaleTheoTime(float scale)
    {
        StartCoroutine(ScaleTheoTime(scale));
    }
    IEnumerator ScaleTheoTime(float scale)
    {
        float timeDelay = 0.1f;
        while(transform.localScale.x < scale)
        {
            transform.localScale += transform.localScale * scale * Time.deltaTime;
            yield return new WaitForSeconds(timeDelay);
        }
        yield return null;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void UnApplyScale(int src)
    {
        float scale = 1f;
        for (int i = listSrc.Count - 1; i >= 0; --i)
        {
            var item = listSrc[i];
            if (item.Src == src)
            {
                listSrc.RemoveAt(i);
            }
            else
            {
                scale *= item.Scale;
            }
        }

        ScaleTheoTile(scale);
    }

    public void ResetScale()
    {
        listSrc.Clear();
        ScaleTheoTile(1f);
    }
}

public struct DanToSrc
{
    public int Src;
    public float Scale;
}