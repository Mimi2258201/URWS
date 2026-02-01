using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalRangedWeaponSystem
{
    public class Pool<T> : IDisposable where T : Component
    {
        public int maxCount { get; private set; }
        public T prefab { get; private set; }
        private Queue<T> pool;

        public Pool(T Object, int PoolSize)
        {
            maxCount = PoolSize;
            pool = new Queue<T>();
            prefab = Object;
        }

        // Destructor so that objects are not forgotten about and Pool can be safely overwritten
        // Not sure this interface works properly so this is just called manually when RangedWeapon is destroyed which is easier as it's a monobehaviour
        public void Dispose()
        {
            foreach (var item in pool)
            {
                if (item != null)
                    GameObject.Destroy(item.gameObject);
            }
        }

        public T SpawnObject(Vector3 position, Quaternion rotation)
        {
            T newObject = default;
            if (prefab != null)
            {
                // Pool not full, spawn new items
                if (pool.Count < maxCount)
                {
                    newObject = GameObject.Instantiate(prefab, position, rotation);
                    pool.Enqueue(newObject);                    
                }

                // Pool full, start reusing
                else
                {
                    newObject = pool.Dequeue();
                    newObject.transform.position = position;
                    newObject.transform.rotation = rotation;
                    newObject.gameObject.SetActive(true);
                    pool.Enqueue(newObject);
                }
            }
            else
                Debug.LogError("Pool not correctly setup, no prefab.");

            return newObject;
        }
    
    }
}
