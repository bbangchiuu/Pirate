using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ANTICHEAT
using Int64 = CodeStage.AntiCheat.ObscuredTypes.ObscuredLong;
using Int32 = CodeStage.AntiCheat.ObscuredTypes.ObscuredInt;
using Float = CodeStage.AntiCheat.ObscuredTypes.ObscuredFloat;
using Bool = CodeStage.AntiCheat.ObscuredTypes.ObscuredBool;
using ObString = CodeStage.AntiCheat.ObscuredTypes.ObscuredString;
#else
using Int64 = System.Int64;
using Int32 = System.Int32;
using Float = System.Single;
using Bool = System.Boolean;
using ObString = System.String;
#endif

public class NodeLevelConfig : MonoBehaviour
{
    //public int HPScale = 100;
    //public int DMGScale = 100;
    public string[] Levels;
    public ENodeType LevelType;

    public ObString RuongConfig;
    public ObString BossCfg;

    private void OnDrawGizmos()
    {
        switch (LevelType)
        {
            case ENodeType.Start:
                Gizmos.DrawIcon(transform.position, "startNode.png", false);
                break;
            case ENodeType.Boss:
                Gizmos.DrawIcon(transform.position, "bossNode.png", false);
                break;
            case ENodeType.Angel:
                Gizmos.DrawIcon(transform.position, "angleNode.png", false);
                break;
            case ENodeType.Ruong:
                Gizmos.DrawIcon(transform.position, "ruongNode.png", false);
                break;
            case ENodeType.Quai1:
                Gizmos.DrawIcon(transform.position, "quai1Node.png", false);
                break;
            case ENodeType.Quai2:
                Gizmos.DrawIcon(transform.position, "quai2Node.png", false);
                break;

            default:
                Gizmos.DrawIcon(transform.position, "dot.png", false);
                break;
        }
    }
}