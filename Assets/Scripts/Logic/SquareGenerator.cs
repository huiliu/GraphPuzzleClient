using System;
using System.Collections.Generic;

namespace GraphGame.Logic
{
    public class Square
    {
        public List<Color> Nodes { get; private set; }
        public Square() { this.Nodes = new List<Color>((int)Color.Max); }

        public void Reset()
        {
            this.Nodes.Clear();
        }

        public void AppendColor(Color color)
        {
            this.Nodes.Add(color);
        }

        public void RemoveColor(int idx)
        {
            this.Nodes[idx] = Color.None;
        }

        public override string ToString()
        {
            var s = "Square Color: ";
            foreach (var c in this.Nodes)
                s += c.ToString() + ", ";

            return s.TrimEnd(' ', ',');
        }
    }

    public class SquareGenerator
    {
        private Dictionary<Color, int> ColorWeight = new Dictionary<Color, int>();
        private List<int> Weights = new List<int>();
        private List<Color> Colors = new List<Color>();
        private int TotalValue = 0;
        private int SquareCount = 0;
        private Random random;
        private const int kSquareEdgeCount = 4;
        private int CurrentIndex = 0;
        private List<List<Color>> ColorSource = new List<List<Color>>(kSquareEdgeCount);   // 因为只有四条边

        /// 各色彩权重字典
        /// None: 100, Red: 500, Green: 200, Blue: 400
        /// 最终权重: 
        /// 
        public SquareGenerator(Dictionary<Color, int> colorWeight, int count)
        {
            this.SquareCount = count;
            this.ColorWeight = colorWeight;
            this.random = new Random(0);

            this.InitWeight();
        }

        private void InitWeight()
        {
            foreach (var kvp in this.ColorWeight)
            {
                this.TotalValue += kvp.Value;
                this.Colors.Add(kvp.Key);
                this.Weights.Add(this.TotalValue);
            }

            for (var i = 0; i < kSquareEdgeCount; ++i)
                this.ColorSource.Add(new List<Color>(this.SquareCount));

            for (var i = 0; i < this.SquareCount; ++i)
            {
                foreach (var cs in this.ColorSource)
                    cs.Add(this.GetColor(random.Next(0, this.TotalValue)));
            }
        }

        private Color GetColor(int w)
        {
            for (var i = 0; i < this.Weights.Count; ++i)
            {
                if (w < this.Weights[i])
                    return this.Colors[i];
            }

            return Color.None;
        }

        public bool IsEmpty { get { return this.CurrentIndex >= this.SquareCount; } }

        private Square Square = new Square();
        public Square GetSquare()
        {
            this.Square.Reset();

            foreach (var sc in this.ColorSource)
                this.Square.AppendColor(sc[this.CurrentIndex]);

            ++this.CurrentIndex;
            return this.Square;
        }
    }
}