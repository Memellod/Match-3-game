using System.Collections.Generic;
using UnityEngine;

namespace ObjectPooling
{
    public interface IPoolable
    {
        void ResetState();

        GameObject GetGO();

    }
    public class ObjectPool
    {
        List<GameObject> pooledObjectList;

        #region constructors
        public ObjectPool(int number, GameObject go)
        {
            pooledObjectList = new List<GameObject>(number);

            for (int i = 0; i < number; i++)
            {
                GameObject obj = Object.Instantiate(go);
                pooledObjectList.Add(obj);
                foreach (IPoolable poolable in obj.GetComponents<IPoolable>())
                {
                    poolable.ResetState();
                }
                obj.SetActive(false);
            }    

        }

        public ObjectPool()
        {
            pooledObjectList = new List<GameObject>();
        }

        #endregion


        public GameObject GetObject()
        {
            if (pooledObjectList.Count == 0) return default;

            GameObject objectToReturn = pooledObjectList[0];
            pooledObjectList.Remove(objectToReturn);
            objectToReturn.SetActive(true);
            return objectToReturn;
        }

        public void AddObject(GameObject obj)
        {
            pooledObjectList.Add(obj);

            foreach (IPoolable poolable in obj.GetComponents<IPoolable>())
            {
                poolable.ResetState();
            }
            obj.transform.SetParent(GameManager.Instance.transform.root);
            obj.SetActive(false);
        }
    }
}
