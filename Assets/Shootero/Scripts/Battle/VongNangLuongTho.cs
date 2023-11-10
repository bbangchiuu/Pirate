using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VongNangLuongTho : MonoBehaviour
{
    public float rotationSpeed;
    public List<GameObject> listObjects;

    private void Awake()
    {
        Setup(0);
    }

    public void Setup(int num)
    {
        for (int i = listObjects.Count; i < num; ++i)
        {
            var t = Instantiate(listObjects[0], transform);
            t.transform.localPosition = Vector3.zero;
            listObjects.Add(t);
        }

        for (int i = 0; i < listObjects.Count; ++i)
        {
            listObjects[i].gameObject.SetActive(i < num);
            if (i < num)
            {
                listObjects[i].transform.localEulerAngles = new Vector3(0, i * 360 / num, 0);
            }
        }
    }

    private void Update()
    {
        transform.Rotate(transform.up, rotationSpeed * Time.deltaTime);
    }
}
