using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphGame.Logic;

namespace GraphGame.Client
{
    public enum GameStatus
    {
        Stop,
        Running,
        Pause,
    }

    public class Bootstrap
        : MonoBehaviour
    {
        public static Bootstrap Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            this.Init();
        }

        private void Init()
        {
            ResourceMgr.Instance.Init();
            ConfigMgr.Instance.Init();
        }

        #region Test
        public void TestConfig()
        {
            ConfigMgr.Instance.TestConfig();
        }
        #endregion
    }
}
