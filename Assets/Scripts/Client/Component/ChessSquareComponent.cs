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

        public int ID;
        private int r;
        private int c;
        public void SetID(int id)
        {
            this.ID = id;
            this.CalcGraphRowCol();
            this.Refresh();
        }

        private IList<Logic.Color> LastColors;
        private void Refresh()
        {
            this.LastColors = Bootstrap.Instance.Game.GetSquareColor(this.ID);
            this.DoRefresh(this.LastColors);
        }

        private void Update()
        {
            if (Bootstrap.Instance.GameStatus == GameStatus.Running)
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

            Bootstrap.Instance.Game.Ack(Bootstrap.SinglePlayer, r, c);
        }

        private void CalcGraphRowCol()
        {
            c = this.ID % Bootstrap.Instance.Game.GraphWidth;
            r = (this.ID - c) / Bootstrap.Instance.Game.GraphWidth;
        }
    }
}
