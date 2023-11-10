using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ChargeStatusEff
{
    public int ID { get; set; }
    public float CountStat { get; set; }
    public float MaxCount { get; set; }
}

public enum ChargeEff
{
    RuningCharge
}