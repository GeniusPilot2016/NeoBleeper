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

    public SavedStateMemento CreateSavedStateMemento(int bpmValue, int alternatingNoteLength)
    {
        var items = listView.Items.Cast<ListViewItem>().Select(item => (ListViewItem)item.Clone()).ToList();
        return new SavedStateMemento(items, bpmValue, alternatingNoteLength);
    }

    public void SetMemento(Memento memento)
    {
        if (memento == null)
            throw new ArgumentNullException(nameof(memento), "Memento cannot be null");

        if (memento.Items == null)
            throw new ArgumentException("Memento items cannot be null", nameof(memento));

        listView.Items.Clear();
        foreach (var item in memento.Items)
        {
            listView.Items.Add((ListViewItem)item.Clone());
        }
    }
}
