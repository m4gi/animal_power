using System.Collections.Generic;
using Tuns.Base;
using UnityEngine;

namespace Game.Scripts
{
    public class PooledObject : MonoBehaviour
    {
        private ObjectPool pool;

        public void SetPool(ObjectPool objectPool)
        {
            pool = objectPool;
        }

        public void ReturnToPool()
        {
            if (pool != null)
            {
                gameObject.SetActive(false);
                pool.ReturnToPoolInternal(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}