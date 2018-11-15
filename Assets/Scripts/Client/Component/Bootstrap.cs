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
        private GameComponent GameComponent;
        [SerializeField]
        private int GameBoardWidth = 3;
        [SerializeField]
        private int GameBoardHeight = 3;

        public static string SinglePlayer = "xyz";
        public static Bootstrap Instance { get; private set; }

        public Game Game { get { return this.GameComponent.Game; } }
        public GameStatus GameStatus { get; private set; }

        private void Awake()
        {
            Instance = this;

            this.GameStatus = GameStatus.Stop;
        }

        public void StartGame(int level)
        {
            this.GameComponent.StartGame(this.GameBoardWidth, this.GameBoardHeight);
            this.GameComponent.AddPlayer(SinglePlayer);
            this.GameStatus = GameStatus.Running;
        }

        public void TerminateGame()
        {
            this.GameComponent.Terminate();
            this.GameStatus = GameStatus.Stop;
        }

        public void Pause()
        {
            this.GameStatus = GameStatus.Pause;
        }

        public void Resume()
        {
            this.GameStatus = GameStatus.Running;
        }
    }
}
