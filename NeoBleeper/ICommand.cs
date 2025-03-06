using System.ComponentModel;

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
        try
        {
            listView.Items.Add(item);
        }
        catch (InvalidAsynchronousStateException)
        {
            return;
        }
    }

    public void Undo()
    {
        try
        {
            listView.Items.Remove(item);
        }
        catch (InvalidAsynchronousStateException)
        {
            return;
        }
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

    public async void Execute()
    {
        try
        {
            listView.Items.Remove(item);
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
            listView.Items.Add(item);
        }
        catch (InvalidAsynchronousStateException)
        {
            return;
        }
    }
}
