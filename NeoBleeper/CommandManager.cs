// NeoBleeper - AI-enabled tune creation software using the system speaker (aka PC Speaker) on the motherboard
// Copyright (C) 2023 GeniusPilot2016
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

public class CommandManager
{
    private Stack<ICommand> undoStack = new Stack<ICommand>();
    private Stack<ICommand> redoStack = new Stack<ICommand>();
    private Stack<Memento> undoMementos = new Stack<Memento>();
    private Stack<Memento> redoMementos = new Stack<Memento>();
    private Originator originator;
    private Memento initialMemento;

    public CommandManager(Originator originator)
    {
        this.originator = originator;
        this.initialMemento = originator.CreateMemento();
    }

    public bool CanUndo => undoStack.Count > 0;
    public bool CanRedo => redoStack.Count > 0;

    public event EventHandler StateChanged;

    public void ExecuteCommand(ICommand command)
    {
        undoMementos.Push(originator.CreateMemento());
        command.Execute();
        undoStack.Push(command);
        redoStack.Clear();
        redoMementos.Clear();
        OnStateChanged();
    }

    public void ClearHistory()
    {
        undoStack.Clear();
        redoStack.Clear();
        undoMementos.Clear();
        redoMementos.Clear();
        initialMemento = originator.CreateMemento();
        OnStateChanged();
    }

    public void Undo()
    {
        if (CanUndo)
        {
            ICommand command = undoStack.Pop();
            redoMementos.Push(originator.CreateMemento());
            command.Undo();
            redoStack.Push(command);
            originator.SetMemento(undoMementos.Pop());
            OnStateChanged();
        }
    }

    public void Redo()
    {
        if (CanRedo)
        {
            ICommand command = redoStack.Pop();
            undoMementos.Push(originator.CreateMemento());
            command.Execute();
            undoStack.Push(command);
            originator.SetMemento(redoMementos.Pop());
            OnStateChanged();
        }
    }

    protected virtual void OnStateChanged()
    {
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsAtInitialState()
    {
        return undoStack.Count == 0 && redoStack.Count == 0 && originator.CreateMemento().Equals(initialMemento);
    }
}
