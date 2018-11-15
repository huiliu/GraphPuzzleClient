using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GraphGame.Client
{
    public class GameOverDialogComponent
        : MonoBehaviour
    {
        [SerializeField] private Text FinalScoreText;

        public void SetScore(int s)
        {
            this.FinalScoreText.text = s.ToString();
        }
    }
}
