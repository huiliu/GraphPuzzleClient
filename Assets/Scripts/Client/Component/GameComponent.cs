using System;
using System.Collections;
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
        [SerializeField] private BubbleScoreComponent BubbleScore;
        [SerializeField] private float PathShowTimePerStep;
        // 游戏结束时对话框延时显示
        [SerializeField] private float DelayShowGameOverDialogTimeS = 0.5f;

        private LinePathComponent LinePathComponent;
        private RectTransform GameBoardRect;
        private void Awake()
        {
            this.GameBoardRect = this.GameBoardNode.GetComponent<RectTransform>();
            this.LinePathComponent = this.LinePathObject.GetComponent<LinePathComponent>();
            this.LinePathComponent.OnArriveNode += this.OnArriveNode;
            this.GameStatus = GameStatus.Stop;
            this.WaitForSeconds = new WaitForSeconds(this.DelayShowGameOverDialogTimeS);

            GameObjectPool.Instance.Registe(BubbleScoreComponent.kPoolName, 5, 2, this.CreateBubbleScore);
        }

        private void OnDestroy()
        {
            GameObjectPool.Instance.UnRegiste(BubbleScoreComponent.kPoolName);
        }

        public static string SinglePlayer = "xyz";
        public LevelData Cfg { get; private set; }
        public GameBoard Game { get; private set; }
        public GameStatus GameStatus { get; private set; }
        private int CurrentLevelID;
        public void StartGame(int levelID)
        {
            this.CurrentLevelID = levelID;

            this.GameBoardNode.GetComponent<GameBoardComponent>().Setup(this);

            this.Cfg = ConfigMgr.Instance.GetLevelConfig(this.CurrentLevelID);
            this.Game = new GameBoard();
            this.Game.Init(this.Cfg);
            this.Game.OnGameOver += this.OnGameOver;
            this.Game.OnSquareAck += OnGameSquareAck;

            this.gameObject.SetActive(true);
            this.GameOverDialogComponent.gameObject.SetActive(false);
            this.AddPlayer(SinglePlayer);
            this.GameStatus = GameStatus.Running;
        }

        public void Restart()
        {
            this.Reset();
            this.StartGame(this.CurrentLevelID);
        }

        public void Terminate()
        {
            this.Reset();
            this.gameObject.SetActive(false);
        }

        public void Pause()
        {
            this.GameStatus = GameStatus.Pause;
        }

        public void Resume()
        {
            this.GameStatus = GameStatus.Running;
        }

        public void Reset()
        {
            this.GameStatus = GameStatus.Stop;
            this.Game.OnGameOver -= this.OnGameOver;
            this.Game.OnSquareAck -= OnGameSquareAck;
            if (this.Paths != null)
                this.Paths.Clear();

            this.LinePathComponent.gameObject.SetActive(false);
        }

        private string CurrentPlayer;
        public void AddPlayer(string uid)
        {
            this.CurrentPlayer = uid;
            this.Game.Start(this.CurrentPlayer);
        }

        private void OnGameOver()
        {
            this.StartCoroutine(this.GameOverImpl());
        }

        private WaitForSeconds WaitForSeconds;
        private IEnumerator GameOverImpl()
        {
            do
            {
                if (this.Paths == null || this.Paths.Count == 0)
                    break;

                yield return null;
            }
            while (true);

            yield return this.WaitForSeconds;
            Debug.Log(string.Format("Game Over!\n{0}", this.Game.ToString()));
            this.ShowGameOverDialog();
            this.Reset();
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
            this.Timer.text = this.Game.RemainTime.ToString();
        }

        private void RefreshScore()
        {
            this.Score.text = this.Game.GetPlayerScore(SinglePlayer).ToString();
        }

        private void RefreshSquare()
        {
            this.CurrentSquare.Setup(this.Game.CurrentSquare.Nodes);
            if (this.Game.NextSquare != null)
                this.NextSquare.Setup(this.Game.NextSquare.Nodes);
        }
        #endregion

        private void Update()
        {
            if (this.GameStatus == GameStatus.Running)
            {
                //this.Game.Update(Time.deltaTime);
                this.Refresh();
            }
        }

        private void ShowGameOverDialog()
        {
            this.GameOverDialogComponent.SetScore(this.Game.GetPlayerScore(this.CurrentPlayer));
            this.GameOverDialogComponent.gameObject.SetActive(true);
        }

        #region DrawGraphPath
        private Vector2 GameBoardSizeDelta;
        private Queue<Path> Paths;
        private float TimePerNode;
        private int NodeScore;
        private int DrawedLineCount;

        private void OnGameSquareAck()
        {
            this.Paths = this.Game.GetPlayerPath(this.CurrentPlayer);
            this.DrawedLineCount = 0;
            this.DoDrawPath();
        }

        private void GetNodeMoveTime()
        {
            if (this.DrawedLineCount < 15)
                this.TimePerNode = 0.2f;
            else if (this.DrawedLineCount < 80)
                this.TimePerNode = 0.1f;
            else if (this.DrawedLineCount < 500)
                this.TimePerNode = 0.05f;
            else
                this.TimePerNode = 0.02f;
        }

        private Path CurrentPath;
        private List<Point> CurrentPathPoints;
        private void DoDrawPath()
        {
            if (this.Paths.Count == 0)
            {
                this.LinePathObject.SetActive(false);
                return;
            }

            this.GameBoardSizeDelta = this.GameBoardRect.sizeDelta;
            var origin = this.GameBoardSizeDelta / 2;
            origin.x = -origin.x;

            this.CurrentPath = this.Paths.Dequeue();
            this.CurrentPathPoints = this.CurrentPath.Nodes;

            this.LinePathComponent.Reset();
            foreach (var p in this.CurrentPathPoints)
            {
                var v = Vector2.zero;
                v.x = p.Col;
                v.y = -p.Row;

                this.LinePathComponent.AppendNode(origin + v * 16);
            }

            this.GetNodeMoveTime();
            this.DrawedLineCount += this.CurrentPathPoints.Count - 1;
            this.NodeScore = 1;

            LinePathComponent.Run(this.TimePerNode);
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

            var p = this.CurrentPathPoints[idx];
            int baseScore = Utils.CalcScoreStrategy(this.Game.GetPlayerEdgeCount(this.CurrentPlayer, this.CurrentPath.Color, p.Row,p.Col));
            this.NodeScore *= baseScore;
            this.ShowNodeScore(pos, this.NodeScore);
        }

        private void ShowNodeScore(Vector3 pos, int s)
        {
            var go = GameObjectPool.Instance.Rent(BubbleScoreComponent.kPoolName);
            go.GetComponent<BubbleScoreComponent>().SetScore(s);
            go.transform.position = pos;
            go.SetActive(true);
        }

        private BubbleScoreComponent CreateBubbleScore()
        {
            var go = Instantiate(this.BubbleScore.gameObject);
            go.transform.SetParent(this.transform, false);

            return go.GetComponent<BubbleScoreComponent>();
        }
        #endregion
    }
}
