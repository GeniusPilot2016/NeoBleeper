using System.ComponentModel;

public class RewindCommand : ICommand
{
    private Originator originator;
    private Memento initialMemento;
    private Memento currentMemento;

    public RewindCommand(Originator originator, Memento initialMemento)
    {
        this.originator = originator;
        this.initialMemento = initialMemento;
        this.currentMemento = originator.CreateMemento();
    }

    public async void Execute()
    {
        try
        {
            originator.SetMemento(initialMemento);
        }
        catch (InvalidAsynchronousStateException)
        {
            return;
        }
    }

    public async void Undo()
    {
        try
        {
            originator.SetMemento(currentMemento);
        }
        catch (InvalidAsynchronousStateException)
        {
            return;
        }
    }
}