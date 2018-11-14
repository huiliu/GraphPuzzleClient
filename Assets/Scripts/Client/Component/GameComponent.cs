using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using GraphGame.Logic;

namespace GraphGame.Client
{
    public class GameComponent
        : MonoBehaviour
    {
        [SerializeField] private Text Timer;
        [SerializeField] private Text Score;
        [SerializeField] private SquareComponent CurrentSquare;
        [SerializeField] private SquareComponent NextSquare;
        
        private void Refresh()
        {
            this.RefreshTimer();
            this.RefreshScore();
            this.RefreshSquare();
        }

        private void RefreshTimer()
        {
            this.Timer.text = Bootstrap.Instance.Game.RemainTime.ToString();
        }

        private void RefreshScore()
        {
            this.Score.text = Bootstrap.Instance.Game.GetPlayerScore(Bootstrap.SinglePlayer).ToString();
        }

        private void RefreshSquare()
        {
            this.CurrentSquare.Setup(Bootstrap.Instance.Game.CurrentSquare.Nodes);
            if (Bootstrap.Instance.Game.NextSquare != null)
                this.NextSquare.Setup(Bootstrap.Instance.Game.NextSquare.Nodes);
        }

        private void Update()
        {
            if (Bootstrap.Instance.GameStatus == GameStatus.Running)
            {
                this.Refresh();
            }
        }
    }
}
