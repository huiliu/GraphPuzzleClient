using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;
using UnityEngine;

namespace GraphGame.Client
{
    public class ChessSquareComponent
        : SquareComponent
        , IPointerDownHandler
    {
        private List<Logic.Color> LastColors = new List<Logic.Color>((int)Logic.Direction.Max);
        protected override void Awake()
        {
            base.Awake();

            for (var i = 0; i < (int)Logic.Direction.Max; ++i)
                this.LastColors.Add(Logic.Color.None);
        }

        public int ID;
        public void SetID(int id)
        {
            this.ID = id;
            this.CalcGraphRowCol();
            this.Refresh();
        }

        private void Refresh()
        {
            var colors = Bootstrap.Instance.Game.GetSquareColor(this.ID);
            for (var i = 0; i < (int)Logic.Direction.Max; ++i)
                this.LastColors[i] = colors[i];

            this.DoRefresh(this.LastColors);
        }

        private void Update()
        {
            if (EntryComponent.Instance.GameStatus == GameStatus.Running)
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
            if (!this.IsEmpty())
            {
                // TODO: Show Warning Dialog.
                Debug.LogWarning("此位置不可放置！");
                return;
            }

            Bootstrap.Instance.Game.Ack(GameComponent.SinglePlayer, r, c);
        }

        private int r;
        private int c;
        private void CalcGraphRowCol()
        {
            Bootstrap.Instance.Game.IndxConvertToRowCol(this.ID, out r, out c);
        }
    }
}
