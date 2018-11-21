using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Arrow.Base;

namespace GraphGame.Client
{
    public abstract class IPooledMonobehaviour
        : MonoBehaviour
        , IPoolObject
    {
        public abstract void Init();
        public abstract void Release();
    }

    public class GameObjectPool
    {
        private static GameObjectPool instance = new GameObjectPool();
        public static GameObjectPool Instance { get { return instance; } }
        private GameObjectPool() { }

        private Dictionary<string, ObjectPool<IPooledMonobehaviour>> PoolDict = new Dictionary<string, ObjectPool<IPooledMonobehaviour>>();
        public void Init()
        {

        }

        public void Fini()
        {
            foreach (var kvp in this.PoolDict)
                kvp.Value.CleanUp();

            this.PoolDict.Clear();
        }

        public GameObject Rent(string name)
        {
            return this.PoolDict[name].Rent().gameObject;
        }

        public void Return(string name, IPooledMonobehaviour behaviour)
        {
            this.PoolDict[name].Return(behaviour);
        }

        public void Registe(string name, int minLength, int minExtendLength, Func<IPooledMonobehaviour> factory)
        {
            this.PoolDict.Add(name, new ObjectPool<IPooledMonobehaviour>(minLength, minExtendLength, factory));
        }

        public void UnRegiste(string name)
        {
            var p = null as ObjectPool<IPooledMonobehaviour>;
            this.PoolDict.TryGetValue(name, out p);
            if (p != null)
            {
                this.PoolDict.Remove(name);
                p.CleanUp();
            }
        }
    }
}
