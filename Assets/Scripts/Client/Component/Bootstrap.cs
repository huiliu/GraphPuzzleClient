using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphGame.Logic;

namespace GraphGame.Client
{
    public enum GameStatus
    {
        Stop,
        Running,
        Pause,
    }

    public class Bootstrap
        : MonoBehaviour
    {
        [SerializeField]
        private GameObject GameNode;
        [SerializeField]
        private GameOverDialogComponent GameOverDialogComponent;
        [SerializeField]
        private int GameBoardWidth = 3;
        [SerializeField]
        private int GameBoardHeight = 3;

        public static string SinglePlayer = "xyz";
        public static Bootstrap Instance { get; private set; }
        public Game Game { get; private set; }
        public GameStatus GameStatus { get; private set; }

        private void Awake()
        {
            Instance = this;

            this.GameStatus = GameStatus.Stop;
        }

        public void StartGame(int level)
        {
            this.Game = new Game(this.GameBoardWidth, this.GameBoardHeight);
            this.Game.OnGameOver = this.OnGameOver;
            this.Game.Start(SinglePlayer);
            this.GameStatus = GameStatus.Running;

            this.GameOverDialogComponent.gameObject.SetActive(false);
            this.GameNode.SetActive(true);
        }

        public void TerminateGame()
        {
            this.GameStatus = GameStatus.Stop;
            this.ShowGameOverDialog();
        }

        public void Pause()
        {
            this.GameStatus = GameStatus.Pause;
        }

        public void Resume()
        {
            this.GameStatus = GameStatus.Running;
        }

        // Update is called once per frame
        void Update ()
        {
            if (this.GameStatus == GameStatus.Running)
            {
                this.Game.Update(Time.deltaTime);
            }
        }

        private void OnGameOver()
        {
            Debug.Log(string.Format("Game Over!\n{0}", this.Game.ToString()));
            this.ShowGameOverDialog();
        }

        private void ShowGameOverDialog()
        {
            this.GameOverDialogComponent.SetScore(this.Game.GetPlayerScore(SinglePlayer));
            this.GameOverDialogComponent.gameObject.SetActive(true);
        }
    }
}
