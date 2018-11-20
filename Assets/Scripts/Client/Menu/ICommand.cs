using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphGame.Client
{
    public interface ICommand
    {
        void Execute();
    }

    public interface IUndoableCommand
        : ICommand
    {
        void Undo();
    }
}
