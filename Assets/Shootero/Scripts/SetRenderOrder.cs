using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetRenderOrder : MonoBehaviour
{
    public int RenderOrder = 0;

    public MeshRenderer[] listMesh;

    public SkinnedMeshRenderer[] listSkinnedMesh;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < listMesh.Length; i++)
        {
            listMesh[i].sortingOrder = RenderOrder;
        }

        for (int i = 0; i < listSkinnedMesh.Length; i++)
        {
            listSkinnedMesh[i].sortingOrder = RenderOrder;
        }

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < listMesh.Length; i++)
        {
            listMesh[i].sortingOrder = RenderOrder;
        }

        for (int i = 0; i < listSkinnedMesh.Length; i++)
        {
            listSkinnedMesh[i].sortingOrder = RenderOrder;
        }
    }
}
