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

    public void Execute()
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

    public void Undo()
    {
        listView.Items.Remove(newItem);
    }
}
