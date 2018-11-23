using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using GraphGame.Logic;
using UnityEngine;

namespace GraphGame.Client
{
    public class VideoPlayerComponent
        : MonoBehaviour
    {
        [SerializeField] int Speed = 8;
        [SerializeField] GameObject GameNode;
        [SerializeField] GameBoardComponent GameBoardComponent;
        [SerializeField] VideoFileComponent VideoFileComponent;
        [SerializeField] GameObject FileListNode;
        [SerializeField] SquareComponent CurrentSquare;
        [SerializeField] SquareComponent NextSquare;
        [SerializeField] GameObject GameOverNode;
        private const float kStepInvervalS = 8f;

        protected void Awake()
        {
            GameObjectPool.Instance.Registe(VideoFileComponent.PoolName, 2, 2, this.CreateVideoFileComponent);
        }

        protected void OnDestroy()
        {
            GameObjectPool.Instance.UnRegiste(VideoFileComponent.PoolName);
        }

        protected void OnEnable()
        {
            this.InitPlayer();
            this.FileListNode.SetActive(true);
            this.GameNode.SetActive(false);
        }

        protected void OnDisable()
        {
            foreach (var c in this.FileList)
            {
                c.OnSelected -= this.HandleSelectVideoFile;
                GameObjectPool.Instance.Return(VideoFileComponent.PoolName, c);
            }

            this.FileList.Clear();
        }

        #region 播放时操作
        public void SpeedUp()
        {
            this.Speed *= 2;
            this.RefreshStepInterval();
        }

        public void SpeedDown()
        {
            this.Speed /= 2;
            this.RefreshStepInterval();
        }

        private bool isPause;
        public void Pause()
        {
            this.isPause = !this.isPause;
        }
        #endregion

        private List<VideoFileComponent> FileList = new List<VideoFileComponent>();
        private void InitPlayer()
        {
            var list = this.GetVideoList();

            for (var i = 0; i < list.Count; ++i)
            {
                var go = GameObjectPool.Instance.Rent(VideoFileComponent.PoolName);
                var c = go.GetComponent<VideoFileComponent>();
                c.OnSelected += this.HandleSelectVideoFile;
                c.Setup(i, list[i]);
                this.FileList.Add(c);

                go.SetActive(true);
            }
        }

        private List<string> Videos = new List<string>();
        public IList<string> GetVideoList()
        {
            this.Videos.Clear();

            DirectoryInfo directory = new DirectoryInfo(ResourceMgr.Instance.GetVideoFileDir());
            foreach (var fi in directory.GetFiles("*.xml"))
                this.Videos.Add(fi.FullName);

            return this.Videos;
        }

        private float stepIntervalS;
        private bool isPlaying = false;
        private void Play(int idx)
        {
            ResourceMgr.Instance.LoadVideo(this.Videos[idx], this.ParseRecordData);

            this.isPlaying = true;
            this.isPause = false;
            this.accumulateTime = 0;
            this.CurrentStepIndex = 0;

            this.StartGame();
            this.RefreshStepInterval();

            this.FileListNode.SetActive(false);
            this.GameNode.SetActive(true);
            this.RefreshUI();
        }

        private GameBoard Game;
        public void StartGame()
        {
            var levelCfg = ConfigMgr.Instance.GetLevelConfig(this.Data.LevelID);
            this.Game = new GameBoard();
            this.Game.Init(levelCfg, this.Data.Seed);
            this.Game.OnGameOver += this.HandleGameOver;
            this.Game.Start(this.Data.PlayerA, this.Data.PlayerB);

            this.GameBoardComponent.Setup(levelCfg.BoardWidth, levelCfg.BoardHeight, levelCfg.unUsedSquareID);
        }

        private void RefreshStepInterval()
        {
            this.stepIntervalS = kStepInvervalS / this.Speed;
        }

        private float accumulateTime;
        private int CurrentStepIndex;
        protected void Update()
        {
            if (!this.isPlaying)
                return;

            if (this.isPause)
                return;

            if (this.CurrentStepIndex >= this.Data.Steps.Count)
                return;

            this.accumulateTime += Time.deltaTime;
            if (this.accumulateTime < this.stepIntervalS)
                return;

            this.accumulateTime = 0;
            var step = this.Data.Steps[this.CurrentStepIndex];
            this.Game.Ack(step.UID, step.Row, step.Col);
            this.RefreshUI();

            ++this.CurrentStepIndex;
        }

        private void RefreshUI()
        {
            this.GameBoardComponent.Refresh(this.Game);

            this.CurrentSquare.Setup(this.Game.CurrentSquare.Nodes);
            if (this.Game.NextSquare != null)
                this.NextSquare.Setup(this.Game.NextSquare.Nodes);
        }

        private void HandleGameOver()
        {
            this.isPlaying = false;
            this.isPause = false;
            this.Game.OnGameOver -= this.HandleGameOver;

            var component = this.GameOverNode.GetComponent<GameOverDialogComponent>();
            component.SetScore(this.Game.GetPlayerScore(this.Data.PlayerA));

            this.GameOverNode.SetActive(true);
        }

        private VideoFileComponent CreateVideoFileComponent()
        {
            var go = Instantiate(this.VideoFileComponent.gameObject);
            go.transform.SetParent(this.FileListNode.transform, false);

            return go.GetComponent<VideoFileComponent>();
        }

        private void HandleSelectVideoFile(int idx)
        {
            this.Play(idx);
            this.FileListNode.SetActive(false);
        }

        private RecordData Data;
        protected void ParseRecordData(StreamReader sr)
        {
            if (sr == null)
                return;
            var serializer = new XmlSerializer(typeof(RecordData));
            this.Data = serializer.Deserialize(sr) as RecordData;
        }
    }
}
