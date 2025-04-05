public class ListViewItemComparer : IEqualityComparer<ListViewItem>
{
    public bool Equals(ListViewItem x, ListViewItem y)
    {
        if (x == null || y == null)
            return false;

        if (x.SubItems.Count != y.SubItems.Count)
            return false;

        for (int i = 0; i < x.SubItems.Count; i++)
        {
            if (x.SubItems[i].Text != y.SubItems[i].Text)
                return false;
        }

        return true;
    }

    public int GetHashCode(ListViewItem obj)
    {
        return obj.GetHashCode();
    }
}