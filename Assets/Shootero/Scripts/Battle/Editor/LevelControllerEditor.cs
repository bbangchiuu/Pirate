
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;

[CustomEditor(typeof(QuanlyManchoi))]
public class LevelControllerEditor : Editor
{
    QuanlyManchoi levelController;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        levelController = (QuanlyManchoi)target;
        if (GUILayout.Button("Generate Water's visual"))
        {
            Debug.Log("Generate Water's visual");
            GenerateWaterVisual();
        }
    }

    void GenerateWaterVisual()
    {
        Dictionary<string, GameObject> listWaters = GetListWaters();
        foreach (string gridKey in listWaters.Keys)
        {
            string[] strs = gridKey.Split('.');
            int dx = int.Parse(strs[0]);
            int dz = int.Parse(strs[1]);
            string gridType = "";
            for(int i = 0;i < 8; i++)
            {
                string nearbyGridKey = GetNearbyGridKey(i, dx, dz);
                gridType += listWaters.ContainsKey(nearbyGridKey) ? "0" : "1";
            }
            StringBuilder sb = new StringBuilder(gridType);
            if (sb[0] == '1' || sb[2] == '1') sb[1] = '1';
            if (sb[2] == '1' || sb[4] == '1') sb[3] = '1';
            if (sb[4] == '1' || sb[6] == '1') sb[5] = '1';
            if (sb[6] == '1' || sb[0] == '1') sb[7] = '1';
            gridType = sb.ToString();

            SpriteRenderer spriteRenderer = listWaters[gridKey].GetComponentInChildren<SpriteRenderer>();
            string path = "Assets/Shootero/Enviroment2D/" + levelController.WaterSpritePath + ".png";
            Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
            Sprite sprite = null;
            foreach (Object asprite in sprites)
            {
                if(gridType == asprite.name)
                {
                    sprite = (Sprite)(asprite);
                    break;
                }
            }
            Debug.Log(gridKey + "=" + gridType);
            Undo.RecordObject(spriteRenderer.gameObject, "change sprite");
            if(sprite != null) spriteRenderer.sprite = sprite;
            EditorUtility.SetDirty(spriteRenderer.gameObject);
        }
    }

    string GetNearbyGridKey(int _slotIndex, int _dx, int _dz)
    {
        string gridKey = "";
        switch (_slotIndex)
        {
            case 0:
                gridKey = (_dx) + "." + (_dz + 2);
                break;
            case 1:
                gridKey = (_dx + 2) + "." + (_dz + 2);
                break;
            case 2:
                gridKey = (_dx + 2) + "." + (_dz);
                break;
            case 3:
                gridKey = (_dx + 2) + "." + (_dz - 2);
                break;
            case 4:
                gridKey = (_dx) + "." + (_dz - 2);
                break;
            case 5:
                gridKey = (_dx - 2) + "." + (_dz - 2);
                break;
            case 6:
                gridKey = (_dx - 2) + "." + (_dz);
                break;
            case 7:
                gridKey = (_dx - 2) + "." + (_dz + 2);
                break;
        }
        return gridKey;
    }

    Dictionary<string, GameObject> GetListWaters()
    {
        Dictionary<string, GameObject> listWaters = new Dictionary<string, GameObject>();
        Transform grpBlocks = levelController.transform.parent.Find("Block");
        for (int i = 0; i < grpBlocks.childCount; i++)
        {
            Transform child = grpBlocks.GetChild(i);
            if (!child.gameObject.name.Contains("Water")
                || !child.gameObject.activeSelf) continue;

            int dx = Mathf.RoundToInt(child.localPosition.x);
            int dz = Mathf.RoundToInt(child.localPosition.z);
            string gridKey = dx + "." + dz;
            listWaters.Add(gridKey, child.gameObject);
        }

        return listWaters;
    }
}
