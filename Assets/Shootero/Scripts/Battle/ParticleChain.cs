using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleChain : MonoBehaviour
{
    public ParticleSystem[] Layers;
    public ParticleSystemRenderer[] Renderers;

    Transform target = null;

    public float ChainTime { get; private set; }

    public Transform GetTarget() { return target; }

    public void SetTarget(Transform t)
    {
        target = t;

        UpdateChain();

        ChainTime = 0;
    }

    void UpdateChain()
    {
        if (target != null)
        {
            float distance = (target.position - transform.position).magnitude;

            transform.LookAt(target);

            for (int i = 0; i < Layers.Length; i++)
            {
                Renderers[i].lengthScale = -distance / Layers[i].startSize;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateChain();

        ChainTime += Time.deltaTime;
    }
}
