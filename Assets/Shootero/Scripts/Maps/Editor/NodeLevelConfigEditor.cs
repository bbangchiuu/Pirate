using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NodeLevelConfig))]
public class NodeLevelConfigEditor : Editor
{
    private void OnEnable()
    {
        //if (ConfigManager.loaded == false)
        {
            ConfigManager.LoadConfigs_Client();
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var cfg = target as NodeLevelConfig;

        if (string.IsNullOrEmpty(cfg.RuongConfig) == false)
        {
            var parent = cfg.transform.parent;
            if (parent.name.StartsWith("Chapter"))
            {
                string chapIdxStr = parent.name.Substring(7);
                if (int.TryParse(chapIdxStr, out int chapIdx) &&
                    ConfigManager.chapterConfigs.Length > chapIdx - 1 &&
                    chapIdx > 0)
                {
                    if (ConfigManager.chapterConfigs[chapIdx - 1].Chests.ContainsKey(cfg.RuongConfig) == false)
                    {
                        EditorGUILayout.LabelField("WrongRuong", cfg.RuongConfig);
                    }
                }
            }
        }
    }
}
