using System.ComponentModel;

public class ReplaceLengthCommand : ICommand
{
    private ListView listView;
    private string newLength;
    private List<string> previousLengths;

    public ReplaceLengthCommand(ListView listView, string newLength)
    {
        this.listView = listView;
        this.newLength = newLength;
        this.previousLengths = listView.SelectedItems.Cast<ListViewItem>().Select(item => item.SubItems[0].Text).ToList();
    }

    public void Execute()
    {
        foreach (ListViewItem item in listView.SelectedItems)
        {
            item.SubItems[0].Text = newLength;
        }
    }

    public void Undo()
    {
        for (int i = 0; i < listView.SelectedItems.Count; i++)
        {
            listView.SelectedItems[i].SubItems[0].Text = previousLengths[i];
        }
    }
}
