﻿using System;
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
        [SerializeField] private GameObject BubbleScoreObject;

        private LinePathComponent LinePathComponent;
        private RectTransform GameBoardRect;
        private void Awake()
        {
            this.GameBoardRect = this.GameBoardNode.GetComponent<RectTransform>();
            this.LinePathComponent = this.LinePathObject.GetComponent<LinePathComponent>();
            this.LinePathComponent.OnArriveNode += this.OnArriveNode;
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
            this.Paths.Clear();
            this.ShowGameOverDialog();
        }

        private void ShowGameOverDialog()
        {
            this.GameOverDialogComponent.SetScore(this.Game.GetPlayerScore(this.CurrentPlayer));
            this.GameOverDialogComponent.gameObject.SetActive(true);
        }

        private Vector2 GameBoardSizeDelta;
        private Queue<GraphPath> Paths;
        private void OnGameSquareAck()
        {
            this.Paths = this.Game.GetPlayerPath(this.CurrentPlayer);
            this.DoDrawPath();
        }

        private GraphPath CurrentPath;
        private IList<int> CurrentPathPoints;
        private void DoDrawPath()
        {

            do
            {
                if (this.Paths.Count == 0)
                {
                    this.LinePathObject.SetActive(false);
                    return;
                }

                this.CurrentPath = this.Paths.Dequeue();
                this.CurrentPathPoints = this.CurrentPath.Path;

                if (this.CurrentPathPoints.Count > 1)
                    break;
            } while (false);

            this.GameBoardSizeDelta = this.GameBoardRect.sizeDelta;
            var origin = this.GameBoardSizeDelta / 2;
            origin.x = -origin.x;

            this.LinePathComponent.Reset();
            foreach (var idx in this.CurrentPathPoints)
            {
                var r = -1;
                var c = -1;
                var p = Vector2.zero;
                this.Game.IndxConvertToRowCol(idx, out r, out c);
                p.x = c;
                p.y = -r;
                this.LinePathComponent.AppendNode(origin + p * 16);
            }

            LinePathComponent.Run();
            this.LinePathObject.SetActive(true);
        }

        private void OnArriveNode(PathMoveComponent pathMoveComponent, Vector3 pos, int idx)
        {
            if (pathMoveComponent.IsArrivedDst)
            {
                this.DoDrawPath();
                return;
            }

            // 过滤掉非得分结点，偶数节点都是方块的中间点，不会得分
            if (idx % 2 == 0)
                return;

            var nodeID = this.CurrentPathPoints[idx];
            this.ShowNodeScore(pos, this.Game.GetPlayerColorEdgeCount(this.CurrentPlayer, this.CurrentPath.Color, nodeID));
        }

        private void ShowNodeScore(Vector3 pos, int s)
        {
            var score = Instantiate(this.BubbleScoreObject);
            score.GetComponent<Text>().text = "+" + s;
            score.transform.SetParent(this.transform, false);
            score.transform.position = pos;
            score.SetActive(true);
        }
    }
}
