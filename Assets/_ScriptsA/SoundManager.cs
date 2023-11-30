using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip spawnSound;
    public GameObject audioPrefab;

    private AudioSource source;

    List<GameObject> allSFX = new List<GameObject>();

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    private int count = 0;
    public void AddCount()
    {
        count++;
    }
    public void SpawnSound()
    {
        if(count >= 3)
        {
            StartCoroutine(nameof(CreateSoumd));
        }
    }
    public void CreateSFX()
    {
        var sfxItem = Instantiate(audioPrefab);
        if(!allSFX.Contains(sfxItem))
            allSFX.Add(sfxItem);
        var sfxSource = sfxItem.GetComponent<AudioSource>();
        sfxSource.clip = spawnSound;
        sfxSource.Play();

        //StartCoroutine(DestroySound(sfxItem));
    }
    public void PlaySound()
    {
        source.clip = spawnSound;
        source.Play();
    }
    IEnumerator CreateSoumd()
    {

        for (int i = 0; i < count; i++)
        {
            CreateSFX();
            yield return new WaitForSeconds(0.1f);
        }
        count = 0;
        StartCoroutine(nameof(DestroySound));
    }
    IEnumerator DestroySound()
    {
        yield return new WaitForSeconds(1f);
        foreach (var sfx in allSFX)
        {
            Destroy(sfx);
        }
        allSFX.Clear();
    }

}
