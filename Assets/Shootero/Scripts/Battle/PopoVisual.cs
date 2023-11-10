using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopoVisual : MonoBehaviour
{
    public SkinnedMeshRenderer meshRenderer;

    static readonly Color[] colorRange = new Color[4]
    {
        new Color(23 / 255f, 46 / 255f, 179 / 255f),
        new Color(23 / 255f, 96 / 255f, 178 / 255f),
        new Color(23 / 255f, 86 / 255f, 179 / 255f),
        new Color(23 / 255f, 66 / 255f, 179 / 255f),
    };
    static readonly Color[] colorMelee = new Color[4]
    {
        new Color(164 / 255f, 68 / 255f, 13 / 255f),
        new Color(164 / 255f, 78 / 255f, 13 / 255f),
        new Color(164 / 255f, 88 / 255f, 13 / 255f),
        new Color(164 / 255f, 58 / 255f, 13 / 255f),
    };

    public static Dictionary<string, int> ColorMeleeUnits = new Dictionary<string, int>();
    public static Dictionary<string, int> ColorRangeUnits = new Dictionary<string, int>();

    public void SetupVisual(DonViChienDau unit)
    {
        var parrentScale = unit.transform.localScale;
        transform.localScale = new Vector3(0.6f / parrentScale.x, 0.6f / parrentScale.y, 0.6f / parrentScale.z);
        if (unit.IsAir)
        {
            transform.localPosition = Vector3.up * 4.5f;
        }

        if (unit.IsMelee)
        {
            int colorIdx = 0;
            if (ColorMeleeUnits.ContainsKey(unit.UnitName))
            {
                colorIdx = ColorMeleeUnits[unit.UnitName];
            }
            else
            {
                colorIdx = ColorMeleeUnits.Count % colorMelee.Length;
                ColorMeleeUnits.Add(unit.UnitName, colorIdx);
            }
            SetColor(colorMelee[colorIdx]);
        }
        else
        {
            int colorIdx = 0;
            if (ColorRangeUnits.ContainsKey(unit.UnitName))
            {
                colorIdx = ColorRangeUnits[unit.UnitName];
            }
            else
            {
                colorIdx = ColorRangeUnits.Count % colorRange.Length;
                ColorRangeUnits.Add(unit.UnitName, colorIdx);
            }
            SetColor(colorRange[colorIdx]);
        }
    }
    public static void CleanUp()
    {
        ColorMeleeUnits.Clear();
        ColorRangeUnits.Clear();
    }

    void SetColor(Color c)
    {
        meshRenderer.materials[1].SetColor("_Color", c);
    }
}
