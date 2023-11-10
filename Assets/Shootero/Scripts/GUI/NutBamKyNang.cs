using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NutBamKyNang : MonoBehaviour
{
    public Button btnActiveSkillleft;
    public Button btnActiveSkillright;
    public Text lbSkillLeft;
    public Text lbSkillRight;
    public Image skillCDLeft;
    public Image skillCDRight;
    public Image iconSkillLeft;
    public Image iconSkillRight;

    public static NutBamKyNang instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        //gameObject.SetActive(false);
    }
}
