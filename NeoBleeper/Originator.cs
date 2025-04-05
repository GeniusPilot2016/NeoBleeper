using System.ComponentModel;

public class Originator
{
    private ListView listView;

    public Originator(ListView listView)
    {
        this.listView = listView;
    }

    public Memento CreateMemento()
    {
        var items = listView.Items.Cast<ListViewItem>().Select(item => (ListViewItem)item.Clone()).ToList();
        return new Memento(items);
    }

    public void SetMemento(Memento memento)
    {
        listView.Items.Clear();
        foreach (var item in memento.Items)
        {
            listView.Items.Add((ListViewItem)item.Clone());
        }
    }
}
