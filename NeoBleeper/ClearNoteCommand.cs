using System.ComponentModel;

public class ClearNoteCommand : ICommand
{
    private ListView listView;
    private int noteIndex;
    private List<ListViewItem> previousItems;

    public ClearNoteCommand(ListView listView, int noteIndex)
    {
        this.listView = listView;
        this.noteIndex = noteIndex;
        this.previousItems = listView.SelectedItems.Cast<ListViewItem>().Select(item => (ListViewItem)item.Clone()).ToList();
    }

    public async void Execute()
    {
        try
        {
            foreach (ListViewItem item in listView.SelectedItems)
            {
                item.SubItems[noteIndex].Text = string.Empty;
            }
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
            for (int i = 0; i < listView.SelectedItems.Count; i++)
            {
                listView.SelectedItems[i].SubItems[noteIndex].Text = previousItems[i].SubItems[noteIndex].Text;
            }
        }
        catch (InvalidAsynchronousStateException)
        {
            return;
        }
    }
}
