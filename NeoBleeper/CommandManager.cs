public class CommandManager
{
    private Stack<ICommand> undoStack = new Stack<ICommand>();
    private Stack<ICommand> redoStack = new Stack<ICommand>();
    private Stack<Memento> undoMementos = new Stack<Memento>();
    private Stack<Memento> redoMementos = new Stack<Memento>();
    private Originator originator;

    public CommandManager(Originator originator)
    {
        this.originator = originator;
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
}
