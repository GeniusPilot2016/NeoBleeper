public class Memento
{
    public List<ListViewItem> ListViewItems { get; private set; }

    public Memento(List<ListViewItem> listViewItems)
    {
        ListViewItems = listViewItems.Select(item => (ListViewItem)item.Clone()).ToList();
    }
}
