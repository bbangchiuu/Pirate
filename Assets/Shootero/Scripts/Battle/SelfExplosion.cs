using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfExplosion : MonoBehaviour
{
    float Delay = 0.1f;
    public float Radius = 3f;
    long DMG = 1;
    public LayerMask layerTarget;

    public GameObject explodeParticlePrefab;
    public GameObject activeParticlePrefab;

    bool mIsActive = false;
    float countTime = 0;
    public bool IsExploded { get; private set; }

    Collider[] cacheTarget;

    private void Awake()
    {
        cacheTarget = new Collider[20];

        if (layerTarget.value == LayersMan.Team1LegacyHitLayerMask)
        {
            layerTarget.value = LayersMan.Team1HitLayerMask;
        }
        else if (layerTarget.value == LayersMan.Team1OnlyHitLayerMask)
        {
            layerTarget.value = LayersMan.Team1DefaultHitLayerMask;
        }
    }

    public void Activate(float radius, long dmg, int layerMask, float delay = 0)
    {
        mIsActive = true;
        Delay = delay;
        DMG = dmg;
        Radius = radius;
        layerTarget = layerMask;
        IsExploded = false;
        countTime = 0;

        if (activeParticlePrefab)
        {
            var go = Instantiate(activeParticlePrefab, transform.position, transform.rotation, transform);
            go.gameObject.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mIsActive)
        {
            countTime += Time.deltaTime;

            if (countTime >= Delay)
            {
                Explode();
                mIsActive = false;
            }
        }
    }

    List<DonViChienDau> setTargets = new List<DonViChienDau>();

    private void Explode()
    {
        if (explodeParticlePrefab)
        {
            //var go = Instantiate(explodeParticlePrefab);

            var go = ObjectPoolManager.SpawnAutoUnSpawn(explodeParticlePrefab, 3f);

            go.transform.position = transform.position;
            go.transform.rotation = Quaternion.identity;
            go.gameObject.SetActive(true);
        }

        var selfUnit = GetComponent<DonViChienDau>();
        if (selfUnit != null)
        {
            selfUnit.TakeDamage(selfUnit.GetMaxHP(), false, null, false, false);
        }
        else
        {
            gameObject.SetActive(false);
            Destroy(gameObject, 0.5f);
        }

        setTargets.Clear();
        int g = Physics.OverlapSphereNonAlloc(transform.position, Radius, cacheTarget, layerTarget);
        for (int i = 0; i < g; ++i)
        {
            var t = cacheTarget[i];
            var unit = t.gameObject.GetComponent<DonViChienDau>();
            if (unit != null && setTargets.Contains(unit) == false)
            {
                setTargets.Add(unit);
                unit.TakeDamage(DMG, false, null, false, xuyenInvulnerable: true);
            }
        }
        IsExploded = true;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    var otherUnit = other.GetComponent<BattleUnit>();

    //}
}
