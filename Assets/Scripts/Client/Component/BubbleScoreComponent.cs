using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GraphGame.Client
{
    [RequireComponent(typeof(Text))]
    [RequireComponent(typeof(DelayToDoComponent))]
    public class BubbleScoreComponent
        : IPooledMonobehaviour
    {
        public const string kPoolName = "BubbleScore";

        private Text Text;
        private DelayToDoComponent DelayToDoComponent;
        private void Awake()
        {
            this.DelayToDoComponent = this.GetComponent<DelayToDoComponent>();
            this.DelayToDoComponent.OnTimeout += OnTimeout;
        }

        private void OnTimeout(DelayToDoComponent obj)
        {
            GameObjectPool.Instance.Return(kPoolName, this);
            this.gameObject.SetActive(false);
        }

        public override void Init()
        {
            sb.Remove(0, sb.Length);
            if (this.Text == null)
                this.Text = this.GetComponent<Text>();
            this.Text.text = "";
        }

        public override void Release()
        {
        }

        StringBuilder sb = new StringBuilder(8);
        public void SetScore(int s)
        {
            sb.AppendFormat("+{0}", s);
            this.Text.text = sb.ToString();
        }
    }
}
