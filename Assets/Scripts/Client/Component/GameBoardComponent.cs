using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GraphGame.Logic;

namespace GraphGame.Client
{
    public class GameBoardComponent
        : MonoBehaviour
    {
        [SerializeField] private GameObject ChessMan;

        private RectTransform RectTransform;
        private GridLayoutGroup GridLayoutGroup;
        private List<GameObject> ChessMenObject = new List<GameObject>();

        private void Awake()
        {
            this.RectTransform = this.GetComponent<RectTransform>();
            this.GridLayoutGroup = this.GetComponent<GridLayoutGroup>();
        }

        private void OnEnable()
        {
            this.InitBoard();
        }

        private void OnDisable()
        {
            this.CleanBoard();
        }

        /// <summary>
        /// OnEnable之前执行
        /// </summary>
        private GameComponent GameComponent;
        public void Setup(GameComponent gameComponent)
        {
            this.GameComponent = gameComponent;
        }

        private Vector2 sizeDelta = Vector2.zero;
        private void InitBoard()
        {
            var w = this.GameComponent.Cfg.BoardWidth;
            var h = this.GameComponent.Cfg.BoardHeight;

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
                    square.Setup(this.GameComponent, r, c, this.GameComponent.Cfg.IsUnusedSquare(squareID));

                    this.ChessMenObject.Add(go);
                    ++squareID;
                }
            }
        }

        private void CleanBoard()
        {
            foreach (var go in this.ChessMenObject)
                Destroy(go);

            this.ChessMenObject.Clear();
        }
    }
}
