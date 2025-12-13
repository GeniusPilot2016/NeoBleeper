// NeoBleeper - AI-enabled tune creation software using the system speaker (aka PC Speaker) on the motherboard
// Copyright (C) 2023 GeniusPilot2016
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.ComponentModel;

public interface ICommand
{
    void Execute();
    void Undo();
}

/// <summary>
/// Specifies the target state for an operation, such as whether to apply an action to selected items, checked items, or
/// both.
/// </summary>
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

    /// <summary>
    /// Initializes a new instance of the AddNoteCommand class to add a single item to the specified ListView.
    /// </summary>
    /// <param name="listView">The ListView control to which the item will be added. Cannot be null.</param>
    /// <param name="item">The ListViewItem to add to the ListView. Cannot be null.</param>
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

    /// <summary>
    /// Initializes a new instance of the AddNoteCommand class to add multiple ListViewItem objects to a ListView at a
    /// specified position or append them to the end.
    /// </summary>
    /// <remarks>Items are inserted in the order provided. If insertIndex is specified and valid, items are
    /// inserted starting at that index; otherwise, they are appended. The original items are not modified, as clones
    /// are used for insertion.</remarks>
    /// <param name="listView">The ListView control to which the items will be added. Cannot be null.</param>
    /// <param name="items">The collection of ListViewItem objects to add. Each item will be cloned before insertion. Cannot be null.</param>
    /// <param name="insertIndex">The zero-based index at which to begin inserting items. If set to a value less than 0, items are appended to the
    /// end of the ListView.</param>
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

    /// <summary>
    /// Applies the current ListView control's visual style to the specified ListViewItem and its subitems.
    /// </summary>
    /// <remarks>This method ensures that the provided ListViewItem and all its subitems visually match the
    /// parent ListView's appearance. If the ListView control is null, no changes are made.</remarks>
    /// <param name="li">The ListViewItem to which the ListView's background color, foreground color, and font will be applied. Cannot be
    /// null.</param>
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

    /// <summary>
    /// Inserts added items into the list view, placing items with explicit indices at their specified positions and
    /// appending others in their original order.
    /// </summary>
    /// <remarks>Items with a non-negative index are inserted in ascending index order at the specified
    /// positions within the list view. Items with a negative index are appended to the end of the list view in the
    /// order they appear in the added items collection. If an invalid asynchronous state is detected, the operation is
    /// aborted and no items are inserted.</remarks>
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

    /// <summary>
    /// Reverses the most recent add operation by removing the previously added items from the list view.
    /// </summary>
    /// <remarks>If the list view's state has changed asynchronously and the operation cannot be completed,
    /// the method returns without making changes. This method does not throw an exception in such cases.</remarks>

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

    /// <summary>
    /// Applies the current style settings of the associated ListView to the specified ListViewItem and its subitems.
    /// </summary>
    /// <remarks>This method updates the background color, foreground color, and font of the specified item
    /// and all its subitems to match those of the parent ListView. If the ListView or the item is null, no changes are
    /// made.</remarks>
    /// <param name="li">The ListViewItem to which the style settings will be applied. Cannot be null.</param>
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

    /// <summary>
    /// Inserts the specified item into the ListView at the given index, applying the appropriate style before
    /// insertion.
    /// </summary>
    /// <remarks>If the specified index is less than zero, the item is added to the end of the ListView. If
    /// the index exceeds the current number of items, the item is also added to the end.</remarks>
    public void Execute()
    {
        ApplyListViewStyle(item);
        // Guard index bounds
        int insertAt = (index < 0) ? listView.Items.Count : Math.Min(index, listView.Items.Count);
        listView.Items.Insert(insertAt, item);
    }

    /// <summary>
    /// Removes the previously added item from the list view, effectively undoing the last add operation.
    /// </summary>
    public void Undo()
    {
        listView.Items.Remove(item);
    }
}

public class RemoveNoteCommand : ICommand
{
    private ListView listView;
    // Stored as (originalIndex, originalReference, cloneForRestore)
    private List<(int Index, ListViewItem Original, ListViewItem Clone)> removedItems;

    // Existing single-item constructor (keeps backward compatibility)

    /// <summary>
    /// Initializes a new instance of the RemoveNoteCommand class for removing a single item from the specified
    /// ListView.
    /// </summary>
    /// <param name="listView">The ListView control from which the item will be removed. Cannot be null.</param>
    /// <param name="item">The ListViewItem to remove from the ListView. Cannot be null and must belong to the specified ListView.</param>
    public RemoveNoteCommand(ListView listView, ListViewItem item)
    {
        this.listView = listView;
        this.removedItems = new List<(int, ListViewItem, ListViewItem)>
        {
            (item.Index, item, (ListViewItem)item.Clone())
        };
    }

    // Remove by target constructor

    /// <summary>
    /// Initializes a new instance of the RemoveNoteCommand class that targets notes in the specified ListView according
    /// to the given selection criteria.
    /// </summary>
    /// <remarks>The items to be removed are determined at construction time based on the current state of the
    /// ListView and the specified target. Items are recorded in descending index order to ensure correct removal
    /// behavior when executed.</remarks>
    /// <param name="listView">The ListView control containing the items to be considered for removal. Cannot be null.</param>
    /// <param name="target">Specifies which items to target for removal: checked, selected, or both.</param>
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

    /// <summary>
    /// Removes the previously tracked items from the associated ListView control.
    /// </summary>
    /// <remarks>This method attempts to remove all items that were previously marked for removal. If the
    /// operation cannot complete due to an invalid asynchronous state, the method returns without making changes. This
    /// method is typically used to revert or finalize a batch removal operation in a ListView.</remarks>
    public void Execute()
    {
        foreach (var (_, original, _) in removedItems)
        {
            // Remove by reference to ensure correct items are removed even if indices shifted
            listView.Items.Remove(original);
        }
    }

    /// <summary>
    /// Restores previously removed items to their original positions in the list view.
    /// </summary>
    /// <remarks>This method re-inserts all items that were removed in the most recent operation, placing each
    /// item at its original index if possible. If the original index is no longer valid, the item is added to the end
    /// of the list. Call this method to undo a removal action and restore the list view to its prior state.</remarks>
    public void Undo()
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
}