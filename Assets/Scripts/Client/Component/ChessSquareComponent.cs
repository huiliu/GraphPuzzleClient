using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace GraphGame.Client
{
    public class ChessSquareComponent
        : SquareComponent
        , IPointerDownHandler
    {
        private Image BackGround;
        private List<Logic.Color> LastColors = new List<Logic.Color>((int)Logic.Direction.Max);
        protected override void Awake()
        {
            base.Awake();

            this.BackGround = this.GetComponent<Image>();
            for (var i = 0; i < (int)Logic.Direction.Max; ++i)
                this.LastColors.Add(Logic.Color.None);
        }

        private bool UnusedFlag = false;
        private int r, c;
        private GameComponent GameComponent;
        public void Setup(GameComponent gameComponent, int r, int c, bool flag)
        {
            this.GameComponent = gameComponent;
            this.r = r * 2 + 1;
            this.c = c * 2 + 1;
            this.UnusedFlag = flag;
            this.Refresh();
        }

        private void Refresh()
        {
            if (this.UnusedFlag)
            {
                this.BackGround.sprite = this.refs["unAvailable"];
                this.TLImage.gameObject.SetActive(false);
                this.TRImage.gameObject.SetActive(false);
                this.DRImage.gameObject.SetActive(false);
                this.DLImage.gameObject.SetActive(false);
                return;
            }

            var colors = this.GameComponent.Game.GetSquareColor(r, c);
            for (var i = 0; i < (int)Logic.Direction.Max; ++i)
                this.LastColors[i] = colors[i];

            this.DoRefresh(this.LastColors);
        }

        private void Update()
        {
            if (!this.UnusedFlag && this.GameComponent.GameStatus == GameStatus.Running)
                this.Refresh();
        }

        private bool IsEmpty()
        {
            if (this.LastColors == null)
                return true;

            foreach (var c in this.LastColors)
            {
                if (c != Logic.Color.None)
                    return false;
            }

            return true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!this.IsEmpty() || this.UnusedFlag)
            {
                // TODO: Show Warning Dialog.
                Debug.LogWarning("此位置不可放置！");
                return;
            }

            this.GameComponent.Game.Ack(GameComponent.SinglePlayer, r, c);
        }
    }
}
