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
        [SerializeField] private GameOverDialogComponent GameOverDialogComponent;
        [SerializeField] private GameObject GameBoardNode;
        [SerializeField] private GameObject LinePathObject;

        private RectTransform GameBoardRect;
        private void Awake()
        {
            this.GameBoardRect = this.GameBoardNode.GetComponent<RectTransform>();
        }

        public Game Game { get; private set; }
        public void StartGame(int rBlock, int cBlock)
        {
            this.Game = new Game(rBlock, cBlock);
            this.Game.OnGameOver = this.OnGameOver;
            this.Game.OnSquareAck += OnGameSquareAck;

            this.gameObject.SetActive(true);
            this.GameOverDialogComponent.gameObject.SetActive(false);
        }

        public void Terminate()
        {
            this.Game.OnSquareAck -= OnGameSquareAck;
            this.ShowGameOverDialog();
        }

        private string CurrentPlayer;
        public void AddPlayer(string uid)
        {
            this.CurrentPlayer = uid;
            this.Game.Start(this.CurrentPlayer);
        }

#region Refresh
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
#endregion

        private void Update()
        {
            if (Bootstrap.Instance.GameStatus == GameStatus.Running)
            {
                //this.Game.Update(Time.deltaTime);
                this.Refresh();
            }
        }

        private void OnGameOver()
        {
            Debug.Log(string.Format("Game Over!\n{0}", this.Game.ToString()));
            this.ShowGameOverDialog();
        }

        private void ShowGameOverDialog()
        {
            this.GameOverDialogComponent.SetScore(this.Game.GetPlayerScore(this.CurrentPlayer));
            this.GameOverDialogComponent.gameObject.SetActive(true);
        }

        private Vector2 GameBoardSizeDelta;
        private List<GameObject> PathMoveComponents = new List<GameObject>();
        private Dictionary<Logic.Color, List<List<int>>> CurrentColorPath;
        private void OnGameSquareAck()
        {
            this.CurrentColorPath = this.Game.GetPlayerPath(this.CurrentPlayer);
            this.DoDrawPath();

            //var colorPath = this.Game.GetPlayerPath(this.CurrentPlayer);
            //foreach (var kvp in colorPath)
            //{
            //    foreach (var points in kvp.Value)
            //    {
            //        if (points.Count <= 2)
            //            continue;

            //        var go = Instantiate(this.LinePathObject);
            //        go.transform.SetParent(this.transform, false);
            //        go.SetActive(true);

            //        var LinePath = go.GetComponent<LinePathComponent>();
            //        foreach (var idx in points)
            //        {
            //            var r = -1;
            //            var c = -1;
            //            var p = Vector2.zero;
            //            this.Game.IndxConvertToRowCol(idx, out r, out c);
            //            p.x = c;
            //            p.y = -r;
            //            LinePath.AppendNode(origin + p * 16);
            //        }

            //        LinePath.OnArriveNode += this.OnArriveNode;
            //        LinePath.Run();
            //    }
            //}
        }

        private void DoDrawPath()
        {
            this.GameBoardSizeDelta = this.GameBoardRect.sizeDelta;
            var origin = this.GameBoardSizeDelta / 2;
            origin.x = -origin.x;

            var curColor = Logic.Color.None;
            var pathNum = -1;

            foreach (var kvp in this.CurrentColorPath)
            {
                curColor = kvp.Key;
                for (var i = 0; i < kvp.Value.Count; ++i) //points in kvp.Value)
                {
                    var points = kvp.Value[i];
                    if (points.Count <= 2)
                        continue;

                    var go = Instantiate(this.LinePathObject);
                    go.transform.SetParent(this.transform, false);
                    go.SetActive(true);

                    var LinePath = go.GetComponent<LinePathComponent>();
                    foreach (var idx in points)
                    {
                        var r = -1;
                        var c = -1;
                        var p = Vector2.zero;
                        this.Game.IndxConvertToRowCol(idx, out r, out c);
                        p.x = c;
                        p.y = -r;
                        LinePath.AppendNode(origin + p * 16);
                    }

                    LinePath.OnArriveNode += this.OnArriveNode;
                    LinePath.Run();

                    pathNum = i;
                    break;
                }

                if (pathNum != -1)
                    break;
            }

            if (curColor != Logic.Color.None && pathNum != -1)
                this.CurrentColorPath[curColor].RemoveAt(pathNum);
        }

        private void OnArriveNode(PathMoveComponent pathMoveComponent, int idx)
        {
            if (pathMoveComponent.IsArrivedDst)
            {
                pathMoveComponent.OnArriveNode -= this.OnArriveNode;
                Destroy(pathMoveComponent.gameObject);

                this.DoDrawPath();
            }
        }
    }
}
