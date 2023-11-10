using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

public class EnviSceneEditor : EditorWindow
{
    [SerializeField]
    int chapIndex;
    [SerializeField]
    string oldEnvi;
    [SerializeField]
    string newEnvi;
    [SerializeField]
    SceneAsset[] scenes = new SceneAsset[0];

    SerializedObject so;
    SerializedProperty oldEnviProp;
    SerializedProperty newEnviProp;
    SerializedProperty scenesArrayProp;
    SerializedProperty chapProp;

    private void OnEnable()
    {
        if (so == null)
        {
            so = new SerializedObject(this);
            scenesArrayProp = so.FindProperty("scenes");
            oldEnviProp = so.FindProperty("oldEnvi");
            newEnviProp = so.FindProperty("newEnvi");
            chapProp = so.FindProperty("chapIndex");
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.PropertyField(chapProp);
        EditorGUILayout.PropertyField(oldEnviProp);
        EditorGUILayout.PropertyField(newEnviProp);
        //oldEnvi = EditorGUILayout.TextField("Old Envi", oldEnvi);
        //newEnvi = EditorGUILayout.TextField("New Envi", newEnvi);
        EditorGUILayout.PropertyField(scenesArrayProp, true);
        //if (scenes == null) scenes = new SceneAsset[0];
        //int size = scenes.Length;
        //size = EditorGUILayout.IntField("Scenes", size);
        //while (size < scenes.)
        //if (size != scenes.Length)
        //{
        //    System.Array.Resize(ref scenes, size);
        //}
        //if (size > 0)
        //{
        //    EditorGUI.indentLevel++;
        //}
        //for (int i = 0; i < size; ++i)
        //{
        //    scenes[i] = (SceneAsset)EditorGUILayout.ObjectField(i.ToString(), scenes[i], typeof(SceneAsset), false);
        //}
        so.ApplyModifiedProperties();
        if (GUILayout.Button("Replace"))
        {
            ReplaceScenes();
        }
    }

    [MenuItem("Shootero/Environment Scene No use skybox")]
    private static void TurnoffSkybox()
    {
        var sceneSetup = UnityEditor.SceneManagement.EditorSceneManager.GetSceneManagerSetup();
        var scenes = UnityEditor.EditorBuildSettings.scenes;

        bool cancel = EditorUtility.DisplayCancelableProgressBar("Modifying Scene Environment", "", 0);
        if (cancel)
        {
            Debug.Log("Canceled Environment Scene Editor");

            UnityEditor.SceneManagement.EditorSceneManager.RestoreSceneManagerSetup(sceneSetup);

            EditorUtility.ClearProgressBar();
            return;
        }
        for (int i = 0; i < scenes.Length; ++i)
        {
            var scenePath = scenes[i].path;
            cancel = EditorUtility.DisplayCancelableProgressBar("Modifying Scene Environment", "Process " + scenePath, (float)i / scenes.Length);
            if (cancel)
            {
                Debug.Log("Canceled Environment Scene Editor");

                UnityEditor.SceneManagement.EditorSceneManager.RestoreSceneManagerSetup(sceneSetup);

                EditorUtility.ClearProgressBar();
                return;
            }
            var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);

            Lightmapping.lightingSettings = AssetDatabase.LoadAssetAtPath<LightingSettings>("Assets/Shootero/Scenes/SampleSceneSettings.lighting");

            RenderSettings.skybox = null;
            RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
            RenderSettings.customReflection = null;

            Lightmapping.Bake();

            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
            Debug.Log("Done " + scenePath);
        }

        UnityEditor.SceneManagement.EditorSceneManager.RestoreSceneManagerSetup(sceneSetup);

        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Shootero/Environment Scene Editor")]
    private static void ShowWindow()
    {
        var window = EditorWindow.CreateInstance<EnviSceneEditor>();
        window.titleContent = new GUIContent("Environment Scene Editor");
        window.Show();
    }

    void ReplaceScenes()
    {
        var sceneSetup = UnityEditor.SceneManagement.EditorSceneManager.GetSceneManagerSetup();

        bool cancel = EditorUtility.DisplayCancelableProgressBar("Modifying Scene Environment", "", 0);
        if (cancel)
        {
            Debug.Log("Canceled Environment Scene Editor");
            return;
        }
        for (int i = 0; i < scenes.Length; ++i)
        {
            var sceneAsset = scenes[i];
            if (sceneAsset != null)
            {
                cancel = EditorUtility.DisplayCancelableProgressBar("Modifying Scene Environment", "Process " + sceneAsset.name, (float)i / scenes.Length);
                if (cancel)
                {
                    Debug.Log("Canceled Environment Scene Editor");
                    return;
                }
                string waterPath = string.Format("Chap{0}/water_c{0}", chapIndex);
                var success = ReplaceScene(sceneAsset, oldEnvi, newEnvi, waterPath);
                if (success)
                    Debug.Log("Done " + sceneAsset.name);
                else
                {
                    Debug.Log("Failed " + sceneAsset.name);
                }
                cancel = EditorUtility.DisplayCancelableProgressBar("Modifying Scene Environment", "Done " + sceneAsset.name, (i + 1f) / scenes.Length);
                if (cancel)
                {
                    Debug.Log("Canceled Environment Scene Editor");
                    return;
                }
            }
        }

        UnityEditor.SceneManagement.EditorSceneManager.RestoreSceneManagerSetup(sceneSetup);

        EditorUtility.ClearProgressBar();
    }
    bool ReplaceScene(SceneAsset asset, string oldEnvi, string newEnvi, string waterPath)
    {
        var scenePath = AssetDatabase.GetAssetPath(asset);
        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);

        GenerateWaterVisual(waterPath);
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);

        var oldObj = GameObject.Find("GAMEDESIGN/" + oldEnvi);
        if (oldObj == null)
        {
            Debug.Log(oldEnvi + " not found");
            return false;
        }
        string pathNew = "Assets/Shootero/Prefabs/Enviroment/" + newEnvi + ".prefab";
        var newPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(pathNew);
        if (newPrefab == null)
        {
            Debug.Log(pathNew + " not found");
            return false;
        }
        var newObj = PrefabUtility.InstantiatePrefab(newPrefab, oldObj.transform.parent) as GameObject;
        newObj.name = newEnvi;
        newObj.transform.position = oldObj.transform.position;
        var newTop = newObj.transform.Find("Border/top");
        var oldTop = oldObj.transform.Find("Border/top");
        if (newTop != null && oldTop != null)
        {
            newTop.transform.position = oldTop.transform.position;
        }

        DestroyImmediate(oldObj);
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
        return true;
    }

    void GenerateWaterVisual(string waterPath)
    {
        if (string.IsNullOrEmpty(waterPath)) return;
        Dictionary<string, GameObject> listWaters = GetListWaters();
        foreach (string gridKey in listWaters.Keys)
        {
            string[] strs = gridKey.Split('.');
            int dx = int.Parse(strs[0]);
            int dz = int.Parse(strs[1]);
            string gridType = "";
            for (int i = 0; i < 8; i++)
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
            string path = "Assets/Shootero/Enviroment2D/" + waterPath + ".png";
            Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
            Sprite sprite = null;
            foreach (Object asprite in sprites)
            {
                if (gridType == asprite.name)
                {
                    sprite = (Sprite)(asprite);
                    break;
                }
            }
            Debug.Log(gridKey + "=" + gridType);
            Undo.RecordObject(spriteRenderer.gameObject, "change sprite");
            if (sprite != null) spriteRenderer.sprite = sprite;
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
        var bObj = GameObject.Find("GAMEDESIGN/Block");
        if (bObj == null)
        {
            Debug.Log("BLOCK GRP NOT FOUND");
            return listWaters;
        }
        Transform grpBlocks = bObj.transform;
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
