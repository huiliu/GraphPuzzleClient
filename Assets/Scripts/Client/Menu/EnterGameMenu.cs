using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphGame.Client
{
    public class EnterGameMenu
        : IUndoableCommand
    {
        public void Execute()
        {
            EntryComponent.Instance.ShowLevelNode();
        }

        public void Undo()
        {
            EntryComponent.Instance.HideLevelNode();
        }
    }
}
