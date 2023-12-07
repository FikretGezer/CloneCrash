using System.Collections.Generic;
using UnityEngine;

public class EffectSpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject destroyEffectPrefab;
    [SerializeField] private GameObject iceEffectPrefab;
    [SerializeField] private GameObject rockEffectPrefab;

    private List<GameObject> normalEffects = new List<GameObject>();
    private List<GameObject> iceEffects = new List<GameObject>();
    private List<GameObject> rockEffects = new List<GameObject>();
    private GameObject effectParent;
    public static EffectSpawnManager Instance;
    private void Awake()
    {
        if(Instance == null) Instance = this;
        effectParent = new GameObject();
        effectParent.name = "Effect Parent";

        SpawnEffect(destroyEffectPrefab, normalEffects);
        SpawnEffect(iceEffectPrefab, iceEffects);
        SpawnEffect(rockEffectPrefab, rockEffects);
    }
    private void SpawnEffect(GameObject spawnEffectPrefab, List<GameObject> effectList)
    {
        for (int i = 0; i < 20; i++)
        {
            CreateEffect(spawnEffectPrefab, effectList);
        }
    }
    private GameObject CreateEffect(GameObject spawnEffectPrefab, List<GameObject> effectList)
    {
        var effect = Instantiate(spawnEffectPrefab);

        effect.SetActive(false);
        effect.transform.SetParent(effectParent.transform);

        effectList.Add(effect);
        return effect;
    }
    public GameObject GetNormalEffectFromPool()
    {
        foreach(var effect in normalEffects){
            if(!effect.activeSelf)
                return effect;
        }
        return CreateEffect(destroyEffectPrefab, normalEffects);
    }
    public GameObject GetIceEffectFromPool()
    {
        foreach(var effect in iceEffects){
            if(!effect.activeSelf)
                return effect;
        }
        return CreateEffect(iceEffectPrefab, iceEffects);
    }
    public GameObject GetRockEffectFromPool()
    {
        foreach(var effect in rockEffects){
            if(!effect.activeSelf)
                return effect;
        }
        return CreateEffect(rockEffectPrefab, rockEffects);
    }
}
