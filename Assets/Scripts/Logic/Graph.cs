using System;
using System.Collections.Generic;

namespace GraphGame.Logic
{
    public class GraphNode
    {
        public int ID { get; private set; }
        private HashSet<int> Successors = new HashSet<int>();

        public GraphNode(int id)
        {
            this.ID = id;
        }

        public void AddSuccessor(int id)
        {
            this.Successors.Add(id);
        }

        public void RemoveSuccessor(int id)
        {
            this.Successors.Remove(id);
        }

        public HashSet<int> AllSuccessor
        {
            get { return this.Successors; }
        }

        public override string ToString()
        {
            var s = string.Format("Node[{0}] Successors: ", this.ID);
            foreach(var i in this.Successors)
            {
                s += i.ToString() + ", ";
            }

            return s.TrimEnd(' ', ',');
        }
    }

    public class TraverseRecord
    {
        public IList<int> CurrentNodes { get { return this.Records.AsReadOnly(); } }
        private List<int> Records = new List<int>();
        public int Last
        {
            get
            {
                if (this.Records.Count == 0)
                    return -1;
                return this.Records[this.Records.Count - 1];
            }
        }
        public void Reset()
        {
            this.Records.Clear();
        }

        public void Push(int id)
        {
            this.Records.Add(id);
        }

        public bool HasTraverse(int id)
        {
            var idxs = new List<int>();
            for (var i = 0; i < this.Records.Count; ++i)
            {
                if (this.Records[i] == id)
                    idxs.Add(i);
            }
            if (idxs.Count == 0)
                return false;

            bool traverseFlag = false;
            foreach (var idx in idxs)
            {
                if (this.Records[idx + 1] == this.Last ||
                    (idx > 0 && this.Records[idx - 1] == this.Last))
                    traverseFlag = true;
            }

            return traverseFlag;
        }

        public void Pop()
        {
            this.Records.RemoveAt(this.Records.Count - 1);
        }

        public override string ToString()
        {
            var s = "";
            foreach (var r in this.Records)
                s += r.ToString() + ", ";

            return s.TrimEnd(',', ' ');
        }

    }

    public class Graph
    {
        private List<GraphNode> Nodes;
        public Graph(int count)
        {
            this.Nodes = new List<GraphNode>();
            for (var i = 0; i < count; ++i)
                this.Nodes.Add(new GraphNode(i));
        }

        public void AddEdge(int source, int sink)
        {
            this.Nodes[source].AddSuccessor(sink);
            this.Nodes[sink].AddSuccessor(source);
        }

        public void RemoveEdge(int source, int sink)
        {
            this.Nodes[source].RemoveSuccessor(sink);
            this.Nodes[sink].RemoveSuccessor(source);
        }

        public GraphNode GetNode(int id)
        {
            return this.Nodes[id];
        }

        private List<List<int>> solvers = new List<List<int>>();
        public List<List<int>> Traverse(int rootID)
        {
            this.solvers.Clear();
            this.DFSTraverse(this.GetNode(rootID));

            return this.solvers;
        }

        private TraverseRecord TraverseRecord = new TraverseRecord();
        private bool DFSTraverse(GraphNode node)
        {
            bool isEndNode = true;
            var last = this.TraverseRecord.Last;
            this.TraverseRecord.Push(node.ID);

            do
            {
                foreach (var id in node.AllSuccessor)
                {
                    if (last == id)
                        continue;

                    if (this.TraverseRecord.HasTraverse(id))
                        continue;

                    isEndNode = false;
                    this.DFSTraverse(this.GetNode(id));
                }

                //if (isEndNode)
                //    Console.WriteLine(this.TraverseRecord.ToString());
            }
            while (false);

            if (isEndNode)
                this.solvers.Add(new List<int>(this.TraverseRecord.CurrentNodes));

            this.TraverseRecord.Pop();

            return isEndNode;
        }

        public override string ToString()
        {
            var s = "Graph: \n";
            foreach (var node in this.Nodes)
                s+=node.ToString()+'\n';

            return s;
        }
    }

    public class GraphExample
    {
        public static void Run()
        {
            var graph = new Graph(25);
            graph.AddEdge(4, 8);
            graph.AddEdge(6, 10);
            graph.AddEdge(6, 12);
            graph.AddEdge(8, 12);
            graph.AddEdge(8, 14);
            graph.AddEdge(10, 16);
            graph.AddEdge(12, 16);
            graph.AddEdge(12, 18);
            graph.AddEdge(14, 18);

            graph.Traverse(6);
        }
    }
}