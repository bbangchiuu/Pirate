using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hiker.Networks.Data.Shootero;

public class BattleChest : MonoBehaviour
{
    public Animator animator;

    public void Open()
    {
        if (animator)
            animator.SetTrigger("open");
    }

    private void OnEnable()
    {
        //Hiker.HikerUtils.DoAction(this, () =>
        //{
            
        //}, 0.2f);
    }
}
