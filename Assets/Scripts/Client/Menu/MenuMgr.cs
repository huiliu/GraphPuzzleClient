using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphGame.Client
{
    public class MenuMgr
    {
        private Stack<IUndoableCommand> Menus = new Stack<IUndoableCommand>();
        public MenuMgr() { }

        public void Execute(IUndoableCommand c)
        {
            c.Execute();
            this.Menus.Push(c);
        }

        public void Undo()
        {
            if (this.Menus.Count == 0)
            {
                return;
            }

            var c = this.Menus.Pop();
            c.Undo();
        }
    }
}
