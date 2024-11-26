using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Object Pool Settings")]
    [SerializeField] private GameObject objectPrefab; // The object to pool
    [SerializeField] private int poolSize = 10;       // The initial size of the pool

    private Queue<GameObject> pool = new Queue<GameObject>(); // Queue to hold pooled objects

    private void Start()
    {
        // Initialize the pool with the specified size
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(objectPrefab); // Instantiate the object
            obj.SetActive(false); // Deactivate the object initially
            pool.Enqueue(obj);    // Add the object to the pool
        }
    }

    // Get an object from the pool
    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue(); // Get the first object from the queue
            obj.SetActive(true); // Activate the object
            return obj;
        }
        else
        {
            // If the pool is empty, instantiate a new object
            GameObject obj = Instantiate(objectPrefab);
            obj.SetActive(true); // Activate the newly instantiated object
            return obj;
        }
    }

    // Return an object to the pool
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false); // Deactivate the object before returning it
        pool.Enqueue(obj);    // Add the object back to the pool
    }

    // Clear all pooled objects (useful for cleanup)
    public void ClearPool()
    {
        while (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            Destroy(obj); // Destroy the object to clean up memory
        }
    }
}
