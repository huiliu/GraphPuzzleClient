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
        [SerializeField] GameComponent GameComponent;
        [SerializeField] VideoFileComponent VideoFileComponent;
        [SerializeField] GameObject FileListNode;
        [SerializeField] GameObject OperationNode;
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
            this.OperationNode.SetActive(false);
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
            Data.Load(ResourceMgr.Instance.GetVideoFileFullPath(Videos[idx]));
            ResourceMgr.Instance.LoadVideo(this.Videos[idx], this.ParseRecordData);

            this.isPlaying = true;
            this.isPause = false;
            this.accumulateTime = 0;
            this.CurrentStepIndex = 0;
            this.GameComponent.StartGame(this.Data.LevelID, this.Data.Seed);
            this.RefreshStepInterval();
            this.FileListNode.SetActive(false);
            this.OperationNode.SetActive(true);
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
            this.GameComponent.Game.Ack(step.UID, step.Row, step.Col);

            ++this.CurrentStepIndex;
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
