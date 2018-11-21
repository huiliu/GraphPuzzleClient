using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GraphGame.Logic;

namespace GraphGame.Client
{
    [RequireComponent(typeof(SpriteRef))]
    public class SquareComponent
        : MonoBehaviour
    {
        [SerializeField] protected Image TLImage;
        [SerializeField] protected Image TRImage;
        [SerializeField] protected Image DLImage;
        [SerializeField] protected Image DRImage;

        protected SpriteRef refs;
        protected virtual void Awake()
        {
            this.refs = this.GetComponent<SpriteRef>();
        }

        public void Setup(IList<Logic.Color> colors)
        {
            this.DoRefresh(colors);
        }

        protected void DoRefresh(IList<Logic.Color> colors)
        {
            for (var i = 0; i < (int)Direction.Max; ++i)
            {
                var d = (Direction)i;
                this.SetImage(this.GetSprite(colors[i], d), d);
            }
        }

        private void SetImage(Sprite sprite, Direction direction)
        {
            switch (direction)
            {
                case Direction.TopLeft:
                    this.TLImage.sprite = sprite;
                    break;
                case Direction.TopRight:
                    this.TRImage.sprite = sprite;
                    break;
                case Direction.DownLeft:
                    this.DLImage.sprite = sprite;
                    break;
                case Direction.DownRight:
                    this.DRImage.sprite = sprite;
                    break;
                default:
                    break;
            }
        }

        private Sprite GetSprite(Logic.Color color, Direction direction)
        {
            if (color == Logic.Color.Red)
            {
                if (direction == Direction.TopLeft || direction == Direction.DownRight)
                    return this.refs["RedTL"];
                else
                    return this.refs["RedDL"];
            }

            if (color == Logic.Color.Green)
            {
                if (direction == Direction.TopLeft || direction == Direction.DownRight)
                    return this.refs["GreenTL"];
                else
                    return this.refs["GreenDL"];
            }

            if (color == Logic.Color.Blue)
            {
                if (direction == Direction.TopLeft || direction == Direction.DownRight)
                    return this.refs["BlueTL"];
                else
                    return this.refs["BlueDL"];
            }

            //if (color == Logic.Color.None)
            //{
                if (direction == Direction.TopLeft || direction == Direction.DownRight)
                    return this.refs["NoneTL"];
                else
                    return this.refs["NoneDL"];
            //}
        }
    }
}
