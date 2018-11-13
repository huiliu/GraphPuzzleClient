using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GraphGame.Client
{
    public class SquareComponent
        : MonoBehaviour
    {
        [SerializeField] private Image TLImage;
        [SerializeField] private Image TRImage;
        [SerializeField] private Image DLImage;
        [SerializeField] private Image DRImage;

        public void SetTopLeftImage(Sprite sprite)
        {
            this.TLImage.sprite = sprite;
        }

        public void SetTopRightImage(Sprite sprite)
        {
            this.TRImage.sprite = sprite;
        }

        public void SetDownLeftImage(Sprite sprite)
        {
            this.DLImage.sprite = sprite;
        }

        public void SetDownRightImage(Sprite sprite)
        {
            this.DRImage.sprite = sprite;
        }
    }
}
