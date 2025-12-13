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

    /// <summary>
    /// Executes the specified command and records it for undo and redo operations.
    /// </summary>
    /// <remarks>This method saves the current state before executing the command, enabling the operation to
    /// be undone or redone later. After execution, the redo history is cleared. If the command is null, an exception
    /// may be thrown.</remarks>
    /// <param name="command">The command to execute. Cannot be null.</param>
    public void ExecuteCommand(ICommand command)
    {
        undoMementos.Push(originator.CreateMemento());
        command.Execute();
        undoStack.Push(command);
        redoStack.Clear();
        redoMementos.Clear();
        OnStateChanged();
    }

    /// <summary>
    /// Clears all undo and redo history, resetting the state tracking to the current state.
    /// </summary>
    /// <remarks>After calling this method, all previous undo and redo operations are discarded. The current
    /// state becomes the new baseline for future undo and redo actions.</remarks>
    public void ClearHistory()
    {
        undoStack.Clear();
        redoStack.Clear();
        undoMementos.Clear();
        redoMementos.Clear();
        initialMemento = originator.CreateMemento();
        OnStateChanged();
    }

    /// <summary>
    /// Reverses the most recent operation, restoring the previous state if an undo is available.
    /// </summary>
    /// <remarks>This method has no effect if there are no operations to undo. After calling this method, the
    /// state can be redone using the corresponding redo functionality, if available.</remarks>
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

    /// <summary>
    /// Performs the most recent undone operation, if a redo is available.
    /// </summary>
    /// <remarks>Call this method to reapply the last operation that was undone using the undo functionality.
    /// If there are no operations available to redo, this method has no effect.</remarks>
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

    /// <summary>
    /// Raises the StateChanged event to notify subscribers of a change in state.
    /// </summary>
    /// <remarks>Override this method in a derived class to provide custom logic when the state changes. This
    /// method invokes the StateChanged event with the current instance as the sender and an empty EventArgs
    /// object.</remarks>
    protected virtual void OnStateChanged()
    {
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Determines whether the current state matches the initial state with no undo or redo operations pending.
    /// </summary>
    /// <remarks>Use this method to check if all changes have been undone and the object is in its original
    /// state. This can be useful for enabling or disabling reset or save functionality in user interfaces.</remarks>
    /// <returns>true if the state is unchanged from its initial value and both the undo and redo stacks are empty; otherwise,
    /// false.</returns>
    public bool IsAtInitialState()
    {
        return undoStack.Count == 0 && redoStack.Count == 0 && originator.CreateMemento().Equals(initialMemento);
    }
}
