using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public enum CountState {
    Count,
    Stop
}
public class FlyingCoinsScript : MonoBehaviour
{
    public TMP_Text _t;
    public GameObject coinPrefab;
    public Vector2 flyingPosition {get; private set;}
    public Animator imageAnim;
    public float lerpSpeed = 1f;
    public float countSpeed = 1f;
    public GameObject checkObject;
    public int poolItemCount = 10;
    public int spawnInOnceCount = 5;
    private GameObject coinParent;
    public int count = 0;
    private int actualCount = 0;
    private List<GameObject> coinPool = new List<GameObject>();
    private CountState _countState;
    public static FlyingCoinsScript Instance;
    private void Awake() {
        if(Instance == null) Instance = this;
        if(checkObject != null)
        {
            Debug.Log(Camera.main.ScreenToWorldPoint(checkObject.transform.position));
            flyingPosition = Camera.main.ScreenToWorldPoint(checkObject.transform.position);
        }
        coinParent = new GameObject();
        coinParent.name = "Coin Parent";
        CreatePool();
    }
    private int anotherCount = 0;
    private void Update() {
        if(actualCount < count)
        {
            if(_countState == CountState.Count)
            {
                _countState = CountState.Stop;
                StartCoroutine(Counter());
            }
        }
        if(Input.GetMouseButtonDown(0))
        {
            var pos = Random.insideUnitCircle;
            var obj = GetFromPool();
            obj.transform.position = pos;
        }
        if(Input.GetMouseButtonDown(1))
        {
            for (int i = 0; i < spawnInOnceCount; i++)
            {
                var pos = Random.insideUnitCircle;
                var obj = GetFromPool();
                obj.transform.position = pos;
            }
        }

    }
    public void IncreaseCount()
    {
        count++;
    }
    IEnumerator Counter()
    {
        while(actualCount < count)
        {
            actualCount++;
            _t.text = actualCount.ToString();
            yield return new WaitForSeconds(countSpeed);
        }
        _countState = CountState.Count;
    }

    #region Coin Pooling
    private void CreatePool()
    {
        for (int i = 0; i < poolItemCount; i++)
        {
            var coin = Instantiate(coinPrefab, Vector2.zero, Quaternion.identity);
            coin.transform.SetParent(coinParent.transform);
            coin.SetActive(false);
            coinPool.Add(coin);
        }
    }
    private GameObject GetFromPool()
    {
        GameObject go = null;
        foreach (var coin in coinPool)
        {
            if(!coin.activeInHierarchy)
            {
                go = coin;
                go.transform.position = Vector2.zero;
                break;
            }
        }
        if(go == null)
        {
            go = Instantiate(coinPrefab, Vector2.zero, Quaternion.identity);
            go.transform.SetParent(coinParent.transform);
            coinPool.Add(go);
        }
        else{
            go.SetActive(true);
        }
        return go;
    }
    private void ReturnToThePool(GameObject go)
    {
        go.SetActive(false);
    }
    #endregion
}
