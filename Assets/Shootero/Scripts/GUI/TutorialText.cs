using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    public string TutKey = "";

    public TextMesh text;
    // Start is called before the first frame update
    void Start()
    {
        string key = TutKey + "_" + GameClient.instance.UInfo.Gamer.Name;

        int isShow = PlayerPrefs.GetInt(key, 0);
        //luon bat tut chap 1
        isShow = 0;
        if(isShow==0)
        {
            PlayerPrefs.SetInt(key, 1);

            text.text = Localization.Get(TutKey);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
