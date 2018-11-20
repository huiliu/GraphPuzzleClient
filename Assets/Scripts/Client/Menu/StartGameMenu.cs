using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphGame.Client
{
    public class StartGameMenu
        : IUndoableCommand
    {
        private readonly int levelID;
        public StartGameMenu(int levelID)
        {
            this.levelID = levelID;
        }

        public void Execute()
        {
            EntryComponent.Instance.HideLevelNode();
            EntryComponent.Instance.StartGame(levelID);
        }

        public void Undo()
        {
            EntryComponent.Instance.TerminateGame();
            EntryComponent.Instance.ShowLevelNode();
        }
    }
}
