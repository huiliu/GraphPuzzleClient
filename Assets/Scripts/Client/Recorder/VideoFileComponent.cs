using System;
using UnityEngine;
using UnityEngine.UI;
using GraphGame.Logic;

namespace GraphGame.Client
{
    public class VideoFileComponent
        : IPooledMonobehaviour
    {
        public const string PoolName = "VidioFile";
        public event Action<int> OnSelected;

        private Text Text;
        private void Awake()
        {
            this.GetComponent<Button>().onClick.AddListener(this.HandleClick);
        }

        private int ID;
        public void Setup(int id, string text)
        {
            this.ID = id;
            this.SetText(text);
        }

        private void SetText(string text)
        {
            if (this.Text == null)
                this.Text = this.GetComponentInChildren<Text>();

            this.Text.text = text;
        }

        private void HandleClick()
        {
            this.OnSelected.SafeInvoke(this.ID);
        }

        public override void Init()
        {
            this.ID = -1;
            this.SetText("");
        }

        public override void Release()
        {
        }
    }
}
