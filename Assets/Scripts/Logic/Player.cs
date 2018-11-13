using System.Collections.Generic;

namespace GraphGame.Logic
{
    public class Player
    {
        public string UID { get; private set; }
        public int TotalScore { get; private set; }

        private Dictionary<Color, Graph> graphs = new Dictionary<Color, Graph>();

        private int BoardWidth;
        private int BoardHeight;
        private int NodeCount;
        public Player(string uid, int w, int h)
        {
            this.UID = uid;
            this.BoardWidth = w;
            this.BoardHeight = h;
            this.NodeCount = w * h;
            this.Scores = new Dictionary<Color, int>();
        }

        public void AddColor(Color color)
        {
            this.ColorGraph = new Graph(this.NodeCount);
            this.graphs.Add(color, this.ColorGraph);
            this.Scores.Add(color, 0);
        }

        public void RemoveColor(Color color)
        {
            this.graphs.Remove(color);
            this.Scores.Remove(color);
        }

        public void AddEdge(int r0, int c0, int r1, int c1, Color color)
        {
            var g = this.graphs[color];
            var src = this.GetNodeIndex(r0, c0);
            var dst = this.GetNodeIndex(r1, c1);

            g.AddEdge(src, dst);
        }

        public void RemoveEdge(int r0, int c0, int r1, int c1, Color color)
        {
            var g = this.graphs[color];
            var src = this.GetNodeIndex(r0, c0);
            var dst = this.GetNodeIndex(r1, c1);

            g.RemoveEdge(src, dst);
        }

        // 记录任一颜色的graph，用于查找一个空节点
        private Graph ColorGraph;
        public void GetEmptyNode(out int r, out int c)
        {
            r = c = -1;
            for (var i = 0; i < this.NodeCount; ++i)
            {
                var node = this.ColorGraph.GetNode(i);
                if (node.AllSuccessor.Count > 0)
                    continue;

                var r1 = 0;
                var c1 = 0;
                this.GetRowCol(i, out r1, out c1);
                if (r1 % 2 != 1 || c1 % 2 != 1)
                    continue;

                bool foundFlag = true;
                foreach (var kvp in this.graphs)
                {
                    if (kvp.Value.GetNode(i).AllSuccessor.Count != 0)
                    {
                        foundFlag = false;
                        break;
                    }
                }

                if (foundFlag)
                {
                    r = r1;
                    c = c1;
                    return;
                }
            }
        }

        // color -> score
        public Dictionary<Color, int> Scores { get; private set; }
        public int CalcScore(int r, int c)
        {
            var score = 0;
            foreach (var kvp in this.graphs)
            {
                var s = this.CalcGraphScore(kvp.Value, r, c);
                score += s;
                this.Scores[kvp.Key] += s;
            }

            return score;
        }

        private int CalcGraphScore(Graph g, int r, int c)
        {
            var root = this.GetNodeIndex(r, c);
            var resolvers = g.Traverse(root);

            var score = 0;
            foreach (var s in resolvers)
            {
                int nodeScore = 1;
                foreach (var id in s)
                {
                    var rr = 0;
                    var cc = 0;
                    this.GetRowCol(id, out rr, out cc);
                    if (rr % 2 == 0 && cc % 2 == 0)
                    {
                        var node = g.GetNode(id);
                        nodeScore *= this.CalcScoreStrategy(node.AllSuccessor.Count);
                        score += nodeScore;
                    }
                }

                if (s.Count > 1 && s[0] == s[s.Count - 1])
                    score += this.LoopBufferScore;
            }

            return score;
        }

        private int LoopBufferScore { get { return 2; } }
        /// 节点得分计算策略
        private int CalcScoreStrategy(int count)
        {
            switch (count)
            {
                case 2:
                    return 1;
                case 3:
                    return 2;
                case 4:
                    return 4;
                default:
                    return 0;
            }
        }

        private int GetNodeIndex(int r, int c)
        {
            return r * this.BoardWidth + c;
        }

        private void GetRowCol(int idx, out int r, out int c)
        {
            r = c = -1;

            c = idx % this.BoardWidth;
            r = (idx - c) / this.BoardWidth;
        }

        public override string ToString()
        {
            var s = string.Format("Player[{0}]'s GameBoard: \n", this.UID);
            foreach (var kvp in this.graphs)
            s += string.Format("Color [{0}]:\n{1}", kvp.Key, kvp.Value);

            return s+'\n';
        }
    }
}