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

        private RectTransform RectTransform;
        private GridLayoutGroup GridLayoutGroup;

        private void Awake()
        {
            this.RectTransform = this.GetComponent<RectTransform>();
            this.GridLayoutGroup = this.GetComponent<GridLayoutGroup>();
        }

        private void OnEnable()
        {
            this.InitBoard(this.width, this.height);
        }

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

        private Vector2 sizeDelta = Vector2.zero;
        private List<ChessSquareComponent> Chesses = new List<ChessSquareComponent>();
        private void InitBoard(int w, int h)
        {
            this.sizeDelta.x = w * this.GridLayoutGroup.cellSize.x;
            this.sizeDelta.y = h * this.GridLayoutGroup.cellSize.y;
            this.RectTransform.sizeDelta = this.sizeDelta;

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
