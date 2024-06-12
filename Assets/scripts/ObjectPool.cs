using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject platformPrefab;
    public int initialCount = 20;

    private Queue<GameObject> platforms = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < initialCount; i++)
        {
            GameObject obj = Instantiate(platformPrefab);
            obj.SetActive(false);
            platforms.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (platforms.Count > 0)
        {
            GameObject obj = platforms.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            return Instantiate(platformPrefab); 
        }
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        platforms.Enqueue(obj);
    }
}
