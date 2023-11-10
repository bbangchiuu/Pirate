using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayersMan
{
    public static readonly string[] Team1Layers = new string[]
    {
        "Team1",
        "Team1_Flying",
        "Team1_FlyingWater"
    };

    public static readonly int Team1HitLayerMask = LayerMask.GetMask(Team1Layers);
    public static readonly int Team1DefaultHitLayerMask = LayerMask.GetMask(
        "Team1",
        "Team1_FlyingWater"
        );
    public static readonly int Team1LegacyHitLayerMask = LayerMask.GetMask(
        "Team1",
        "Team1_Flying"
        );
    public static readonly int Team1OnlyHitLayerMask = LayerMask.GetMask("Team1");

    public static readonly int Team1 = LayerMask.NameToLayer("Team1");
    public static readonly int Team1_Flying = LayerMask.NameToLayer("Team1_Flying");
    public static readonly int Team1_FlyingWater = LayerMask.NameToLayer("Team1_FlyingWater");

    public static int GetDefaultHeroLayer(string heroName)
    {
        if (heroName == "IronMan")
        {
            return Team1_FlyingWater;
        }

        return Team1;
    }

    public static bool CheckMask(int layer, int layerMask)
    {
        return ((0x1 << layer) & layerMask) != 0;
    }
}
