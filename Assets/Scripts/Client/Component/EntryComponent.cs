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
        public static EntryComponent Instance { get; private set; }

        [SerializeField]
        private GameObject LevelNode;
        [SerializeField]
        private GameComponent GameComponent;

        private void Awake()
        {
            Instance = this;
            this.MenuMgr = new MenuMgr();
        }

        public Game Game { get { return this.GameComponent.Game; } }
        public GameStatus GameStatus { get { return this.GameComponent.GameStatus; } }
        public void StartGame(int level)
        {
            this.GameComponent.StartGame(level);
        }

        public void TerminateGame()
        {
            this.GameComponent.Terminate();
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

        public void Undo()
        {
            this.MenuMgr.Undo();
        }
    }
}
