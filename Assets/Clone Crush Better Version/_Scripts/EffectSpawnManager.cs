using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject destroyEffectPrefab;

    private List<GameObject> effects = new List<GameObject>();
    private GameObject effectParent;
    private int index = 0;
    public static EffectSpawnManager Instance;
    private void Awake()
    {
        if(Instance == null) Instance = this;
        effectParent = new GameObject();
        effectParent.name = "Effect Parent";

        SpawnEffect();
    }
    private void SpawnEffect()
    {
        for (int i = 0; i < 20; i++)
        {
            CreateEffect();
        }
    }
    private GameObject CreateEffect()
    {
        var effect = Instantiate(destroyEffectPrefab);
        effect.name = "Effect_" + index.ToString();
        effect.SetActive(false);
        effect.transform.SetParent(effectParent.transform);
        effects.Add(effect);
        index++;
        return effect;
    }
    public GameObject GetEffectFromPool()
    {
        foreach(var effect in effects){
            if(!effect.activeSelf)
                return effect;
        }
        return CreateEffect();
    }
}
