using UnityEngine;

public class SpecialTile : MonoBehaviour
{
    public int column;
    public int row;
    [SerializeField] private int health;
    public void TakeDamage()
    {
        health--;
        if(health <= 0)
        {
            DestroyEffect();
            ObjectiveController.Instance.ReduceObjectiveAmount(this.gameObject);
            Destroy(this.gameObject);
        }
    }
    private void DestroyEffect()
    {
        if(tag == "Ice")
        {
            var iceEffect = EffectSpawnManager.Instance.GetIceEffectFromPool();
            SpawnEffect(iceEffect);
        }
        else if(tag == "Breakable")
        {
            var rockEffect = EffectSpawnManager.Instance.GetRockEffectFromPool();
            SpawnEffect(rockEffect);
        }
    }
    private void SpawnEffect(GameObject effect)
    {
        var pos = transform.position;
        effect.transform.position = pos;
        effect.SetActive(true);
    }
}
