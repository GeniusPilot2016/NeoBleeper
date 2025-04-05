public class Memento
{
    public List<ListViewItem> Items { get; }

    public Memento(List<ListViewItem> items)
    {
        Items = items;
    }

    public override bool Equals(object obj)
    {
        if (obj is Memento other)
        {
            return Items.SequenceEqual(other.Items, new ListViewItemComparer());
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Items.GetHashCode();
    }
}