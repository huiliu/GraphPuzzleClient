using System;
using System.Collections.Generic;
using UnityEngine;
using GraphGame.Logic;

namespace GraphGame.Client
{
    public abstract class PathMoveComponent
        : MonoBehaviour
    {
        // 到达某个节点的事件
        public event Action<PathMoveComponent, int> OnArriveNode;

        [SerializeField]
        protected List<Vector2> PathNode = new List<Vector2>();
        [SerializeField]
        protected GameObject BallObject;
        [SerializeField]
        protected float TotalTime;

        protected int CurrentIndex;
        protected float NodeTime;
        protected int NodeCount { get { return this.PathNode.Count; } }

        public void AppendNode(Vector2 pos)
        {
            this.PathNode.Add(pos);
        }

        public void SetNode(int i, Vector2 pos)
        {
            if (i < this.PathNode.Count)
                this.PathNode[i] = pos;
        }

        public void RemoveNode(int i)
        {
            if (i < PathNode.Count)
                this.PathNode.RemoveAt(i);
        }

        public void Reset()
        {
            this.PathNode.Clear();
        }

        protected virtual void OnEnable()
        {
            this.CurrentIndex = 0;
            this.DrawPath();
        }

        protected virtual void Update ()
        {
            if (this.isRunning)
                this.SmoothMove(Time.deltaTime);
        }

        protected abstract void SmoothMove(float dt);

        protected bool isRunning = false;
        public void Run()
        {
            this.isRunning = true;
            this.NodeTime = this.TotalTime / this.NodeCount;
        }

        private void DrawPath()
        {
#if UNITY_EDITOR
            for (var i = 0; i < this.NodeCount -1;++i)
            {
                var s = this.BallObject.transform.TransformPoint(this.PathNode[i]);
                var e = this.BallObject.transform.TransformPoint(this.PathNode[i + 1]);
                Debug.DrawLine(s, e, UnityEngine.Color.green, 10f);
            }
#endif
        }

        protected void FireArriveNode(int i)
        {
            this.OnArriveNode.SafeInvoke(this, this.CurrentIndex);
        }

        public bool IsArrivedDst { get { return this.CurrentIndex == this.NodeCount - 1; } } 
    }
}
