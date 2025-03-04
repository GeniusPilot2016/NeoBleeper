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

    public void Execute()
    {
        originator.SetMemento(initialMemento);
    }

    public void Undo()
    {
        originator.SetMemento(currentMemento);
    }
}