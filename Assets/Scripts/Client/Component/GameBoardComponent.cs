using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GraphGame.Logic;

namespace GraphGame.Client
{
    public class GameBoardComponent
        : MonoBehaviour
    {
        public event Action<int, int> OnClick;
        [SerializeField] private GameObject ChessMan;

        private void OnDisable()
        {
            this.CleanBoard();
        }

        /// <summary>
        /// OnEnable之前执行
        /// </summary>
        private List<int> unusedSquareID;
        private int width;
        private int height;
        public void Setup(int w, int h, List<int> unusedSquareID)
        {
            this.width = w;
            this.height = h;
            this.unusedSquareID = unusedSquareID;

            this.CleanBoard();
            this.InitBoard(this.width, this.height);
        }

        public void Refresh(GameBoard gameBoard)
        {
            foreach (var chess in this.Chesses)
                chess.Refresh(gameBoard);
        }

        private bool IsUnusedSquare(int sid)
        {
            return this.unusedSquareID.FindIndex(item => item == sid) != -1;
        }

        private List<ChessSquareComponent> Chesses = new List<ChessSquareComponent>();
        private void InitBoard(int w, int h)
        {
            var RectTransform = this.GetComponent<RectTransform>();
            var GridLayoutGroup = this.GetComponent<GridLayoutGroup>();
            var sizeDelta = Vector2.zero;

            sizeDelta.x = w * GridLayoutGroup.cellSize.x;
            sizeDelta.y = h * GridLayoutGroup.cellSize.y;
            RectTransform.sizeDelta = sizeDelta;

            var squareID = 0;
            for (var r = 0; r < h; ++r)
            {
                for (var c = 0; c < w; ++c)
                {
                    var go = Instantiate(this.ChessMan);
                    go.transform.SetParent(this.transform, false);
                    go.SetActive(true);

                    var square = go.GetComponent<ChessSquareComponent>();
                    square.OnClick += this.FireSquareClickEvent;
                    square.Setup(r, c, this.IsUnusedSquare(squareID));

                    this.Chesses.Add(square);
                    ++squareID;
                }
            }
        }

        private void CleanBoard()
        {
            foreach (var c in this.Chesses)
            {
                c.OnClick -= this.FireSquareClickEvent;
                Destroy(c.gameObject);
            }

            this.Chesses.Clear();
        }

        private void FireSquareClickEvent(int r, int c)
        {
            this.OnClick.SafeInvoke(r, c);
        }
    }
}
