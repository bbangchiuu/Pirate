using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hiker.GUI;

public class DropMaterialVisual : MonoBehaviour
{
    GameObject visual_obj = null;

    public void SetMaterial(string matName)
    {
        if (visual_obj != null)
        { 
            GameObject.Destroy(visual_obj);
        }

        GameObject mat_obj = Resources.Load("MaterialVisual3D/" + matName) as GameObject;

        if (mat_obj != null)
        {
            visual_obj = Instantiate(mat_obj, this.transform);
            visual_obj.transform.position = Vector3.zero;
        }


        //if (matCollection == null) { matCollection = Resources.Load<SpriteCollection>("MaterialAvatar"); }
        //materialIcon.sprite = matCollection.GetSprite("icon_" + matName);

    }

    public void OnDestroy()
    {
        
    }
}
