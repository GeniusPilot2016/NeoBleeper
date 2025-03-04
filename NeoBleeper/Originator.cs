public class Originator
{
    private ListView listView;

    public Originator(ListView listView)
    {
        this.listView = listView;
    }

    public Memento CreateMemento()
    {
        return new Memento(listView.Items.Cast<ListViewItem>().ToList());
    }

    public void SetMemento(Memento memento)
    {
        listView.Items.Clear();
        foreach (var item in memento.ListViewItems)
        {
            listView.Items.Add((ListViewItem)item.Clone());
        }
    }
}
