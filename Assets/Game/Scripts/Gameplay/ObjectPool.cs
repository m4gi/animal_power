using System.Collections.Generic;
using Tuns.Base;
using UnityEngine;

namespace Game.Scripts
{
    public class ObjectPool : Singleton<ObjectPool>
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        public List<Pool> pools;
        private Dictionary<string, Queue<GameObject>> poolDictionary;

        protected override void AwakeSingleton()
        {
            base.AwakeSingleton();
            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);

                    PooledObject pooledObj = obj.GetComponent<PooledObject>();
                    if (pooledObj == null)
                    {
                        pooledObj = obj.AddComponent<PooledObject>();
                    }
                    pooledObj.SetPool(this);

                    objectPool.Enqueue(obj);
                }
                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
                return null;
            }

            GameObject objectToSpawn = poolDictionary[tag].Dequeue();

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
            if (pooledObj != null)
            {
                pooledObj.OnObjectSpawn();
            }
            
            poolDictionary[tag].Enqueue(objectToSpawn);

            return objectToSpawn;
        }
        
        public void ReturnToPoolInternal(GameObject obj)
        {
            obj.SetActive(false);
        }
    }
    public interface IPooledObject
    {
        void OnObjectSpawn();
    }
}