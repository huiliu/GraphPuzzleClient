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

        private Vector2 sizeDelta = Vector2.zero;
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
            sizeDelta.x = Bootstrap.Instance.Game.ColCount * this.GridLayoutGroup.cellSize.x;
            sizeDelta.y = Bootstrap.Instance.Game.RowCount * this.GridLayoutGroup.cellSize.y;
            this.RectTransform.sizeDelta = this.sizeDelta;

            this.InitBoard();
        }

        private void OnDisable()
        {
            this.CleanBoard();
        }

        private void InitBoard()
        {
            var w = Bootstrap.Instance.Game.ColCount;
            var h = Bootstrap.Instance.Game.RowCount;
            var squareID = 0;
            for (var r = 0; r < h; ++r)
            {
                for (var c = 0; c < w; ++c)
                {
                    var go = Instantiate(this.ChessMan);
                    go.transform.SetParent(this.transform, false);
                    go.SetActive(true);

                    var square = go.GetComponent<ChessSquareComponent>();
                    var id = (2 * w + 1) * (2 * r + 1) + 2 * c + 1;
                    square.Setup(id, Bootstrap.Instance.Game.Cfg.IsUnusedSquare(squareID));

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
