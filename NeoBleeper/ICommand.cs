using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

public interface ICommand
{
    void Execute();
    void Undo();
}

public enum Target
{
    Selected,
    Checked,
    Both
}

public class AddNoteCommand : ICommand
{
    private readonly ListView listView;
    // (Index, Item) - Index == -1 means "append"
    private readonly List<(int Index, ListViewItem Item)> addedItems;

    // Single item constructor (append)
    public AddNoteCommand(ListView listView, ListViewItem item)
    {
        this.listView = listView;
        // clone to preserve the item for redo
        var clone = (ListViewItem)item.Clone();
        addedItems = new List<(int, ListViewItem)> { (-1, clone) };
    }

    // Multiple items constructor.
    // If insertIndex >= 0, items will be inserted starting at insertIndex in given order.
    // If insertIndex < 0, items will be appended in given order.
    public AddNoteCommand(ListView listView, IEnumerable<ListViewItem> items, int insertIndex = -1)
    {
        this.listView = listView;
        addedItems = new List<(int, ListViewItem)>();
        var itemList = items.Select(it => (ListViewItem)it.Clone()).ToList();

        if (insertIndex >= 0)
        {
            // Compute target indices at construction time: insertIndex, insertIndex+1, ...
            int idx = insertIndex;
            foreach (var it in itemList)
            {
                addedItems.Add((idx, it));
                idx++;
            }
        }
        else
        {
            // Append mode: store index=-1 to indicate append
            foreach (var it in itemList)
            {
                addedItems.Add((-1, it));
            }
        }
    }

    private void ApplyListViewStyle(ListViewItem li)
    {
        if (listView == null || li == null) return;

        // Ensure new items match current ListView theme
        li.BackColor = listView.BackColor;
        li.ForeColor = listView.ForeColor;
        li.Font = listView.Font;

        foreach (ListViewItem.ListViewSubItem sub in li.SubItems)
        {
            sub.BackColor = listView.BackColor;
            sub.ForeColor = listView.ForeColor;
            sub.Font = listView.Font;
        }
    }

    public void Execute()
    {
        try
        {
            // Two modes: some items have explicit indices (>=0) -> insert in ascending index order.
            var withIndex = addedItems.Where(a => a.Index >= 0).OrderBy(a => a.Index).ToList();
            var append = addedItems.Where(a => a.Index < 0).ToList();

            // Insert indexed items (ascending) - inserting ascending preserves intended order.
            foreach (var (index, item) in withIndex)
            {
                ApplyListViewStyle(item);
                int insertAt = Math.Max(0, Math.Min(index, listView.Items.Count));
                listView.Items.Insert(insertAt, item);
            }

            // Append remaining items in their original order
            foreach (var (_, item) in append)
            {
                ApplyListViewStyle(item);
                listView.Items.Add(item);
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
            // Remove added items by reference. Do in any order; remove uses object reference.
            foreach (var (_, item) in addedItems)
            {
                listView.Items.Remove(item);
            }
        }
        catch (InvalidAsynchronousStateException)
        {
            return;
        }
    }
}

public class InsertNoteCommand : ICommand
{
    private ListView listView;
    private ListViewItem item;
    private int index;

    public InsertNoteCommand(ListView listView, ListViewItem item, int index)
    {
        this.listView = listView;
        this.item = item;
        this.index = index;
    }

    private void ApplyListViewStyle(ListViewItem li)
    {
        if (listView == null || li == null) return;

        li.BackColor = listView.BackColor;
        li.ForeColor = listView.ForeColor;
        li.Font = listView.Font;

        foreach (ListViewItem.ListViewSubItem sub in li.SubItems)
        {
            sub.BackColor = listView.BackColor;
            sub.ForeColor = listView.ForeColor;
            sub.Font = listView.Font;
        }
    }

    public void Execute()
    {
        try
        {
            ApplyListViewStyle(item);
            // Guard index bounds
            int insertAt = (index < 0) ? listView.Items.Count : Math.Min(index, listView.Items.Count);
            listView.Items.Insert(insertAt, item);
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
            listView.Items.Remove(item);
        }
        catch (InvalidAsynchronousStateException)
        {
            return;
        }
    }
}

public class RemoveNoteCommand : ICommand
{
    private ListView listView;
    // Stored as (originalIndex, originalReference, cloneForRestore)
    private List<(int Index, ListViewItem Original, ListViewItem Clone)> removedItems;

    // Existing single-item constructor (keeps backward compatibility)
    public RemoveNoteCommand(ListView listView, ListViewItem item)
    {
        this.listView = listView;
        this.removedItems = new List<(int, ListViewItem, ListViewItem)>
        {
            (item.Index, item, (ListViewItem)item.Clone())
        };
    }

    // New constructor: remove by target (Selected / Checked / Both)
    public RemoveNoteCommand(ListView listView, Target target)
    {
        this.listView = listView;
        this.removedItems = new List<(int, ListViewItem, ListViewItem)>();

        IEnumerable<ListViewItem> items;
        if (target == Target.Checked)
        {
            items = listView.CheckedItems.Cast<ListViewItem>();
        }
        else if (target == Target.Both)
        {
            items = listView.Items.Cast<ListViewItem>().Where(i => i.Checked || i.Selected).Distinct();
        }
        else
        {
            items = listView.SelectedItems.Cast<ListViewItem>();
        }

        foreach (var item in items)
        {
            removedItems.Add((item.Index, item, (ListViewItem)item.Clone()));
        }

        // Order descending so Execute removes from highest index first (avoid shifting issues)
        removedItems = removedItems.OrderByDescending(r => r.Index).ToList();
    }

    public void Execute()
    {
        try
        {
            foreach (var (_, original, _) in removedItems)
            {
                // Remove by reference to ensure correct items are removed even if indices shifted
                listView.Items.Remove(original);
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
            // Re-insert clones in ascending index order so indices match original layout
            foreach (var (index, _, clone) in removedItems.OrderBy(r => r.Index))
            {
                if (index >= 0 && index <= listView.Items.Count)
                {
                    listView.Items.Insert(index, clone);
                }
                else
                {
                    listView.Items.Add(clone);
                }
            }
        }
        catch (InvalidAsynchronousStateException)
        {
            return;
        }
    }
}