using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GraphGame.Logic;

namespace GraphGame.Client
{
    public class EntryComponent
        : MonoBehaviour
    {
        public static string SinglePlayer = "xyz";
        public static EntryComponent Instance { get; private set; }

        [SerializeField]
        private GameObject LevelNode;
        [SerializeField]
        private GameComponent GameComponent;

        private void Awake()
        {
            Instance = this;
            this.GameStatus = GameStatus.Stop;
            this.MenuMgr = new MenuMgr();
        }

        public Game Game { get { return this.GameComponent.Game; } }
        public GameStatus GameStatus { get; private set; }
        public void StartGame(int level)
        {
            this.GameComponent.StartGame(level);
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

        public MenuMgr MenuMgr { get; private set; }

        #region Level
        public void ShowLevelNode()
        {
            this.LevelNode.SetActive(true);
        }

        public void HideLevelNode()
        {
            this.LevelNode.SetActive(false);
        }
        #endregion

        public void EnterGame()
        {
            this.MenuMgr.Execute(new EnterGameMenu());
        }
    }
}
