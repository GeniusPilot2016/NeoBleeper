using System.ComponentModel;

public class PasteCommand : ICommand
{
    private ListView listView;
    private ListViewItem newItem;
    private int insertIndex;

    public PasteCommand(ListView listView, ListViewItem newItem, int insertIndex)
    {
        this.listView = listView;
        this.newItem = newItem;
        this.insertIndex = insertIndex;
    }

    public async void Execute()
    {
        try
        {
            if (insertIndex >= 0 && insertIndex < listView.Items.Count)
            {
                listView.Items.Insert(insertIndex, newItem);
            }
            else
            {
                listView.Items.Add(newItem);
            }
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
            listView.Items.Remove(newItem);
        }
        catch (InvalidAsynchronousStateException)
        {
            return;
        }
    }
}
