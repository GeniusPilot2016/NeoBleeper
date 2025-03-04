public interface ICommand
{
    void Execute();
    void Undo();
}

public class AddNoteCommand : ICommand
{
    private ListView listView;
    private ListViewItem item;

    public AddNoteCommand(ListView listView, ListViewItem item)
    {
        this.listView = listView;
        this.item = item;
    }

    public void Execute()
    {
        listView.Items.Add(item);
    }

    public void Undo()
    {
        listView.Items.Remove(item);
    }
}

public class RemoveNoteCommand : ICommand
{
    private ListView listView;
    private ListViewItem item;

    public RemoveNoteCommand(ListView listView, ListViewItem item)
    {
        this.listView = listView;
        this.item = item;
    }

    public void Execute()
    {
        listView.Items.Remove(item);
    }

    public void Undo()
    {
        listView.Items.Add(item);
    }
}
