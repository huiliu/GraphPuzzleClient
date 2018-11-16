using System;
using UnityEngine;
using GraphGame.Logic;

namespace GraphGame.Client
{
    public class DelayToDoComponent
        : MonoBehaviour
    {
        public event Action<DelayToDoComponent> OnTimeout;

        [SerializeField]
        private float DelayTime;

        private float time;
        protected virtual void OnEnable()
        {
            this.SetTime(this.DelayTime);
        }

        protected virtual void OnDisable()
        {
            
        }

        public void SetTime(float t)
        {
            this.time = this.DelayTime;
            this.isTimeout = false;
        }

        private bool isTimeout;
        protected virtual void Update()
        {
            this.time -= Time.deltaTime;
            if (!this.isTimeout && time <= 0)
            {
                this.isTimeout = true;
                this.OnTimeout.SafeInvoke(this);
            }
        }
    }
}
