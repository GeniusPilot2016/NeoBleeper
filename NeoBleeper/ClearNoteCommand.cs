using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

public class ClearNoteCommand : ICommand
{
    private readonly ListView listView;
    private readonly int noteIndex;
    private readonly List<(int Index, string PreviousText)> previousStates;
    public enum Target { Selected, Checked, Both }
    private readonly Target target;

    public ClearNoteCommand(ListView listView, int noteIndex, Target target = Target.Selected)
    {
        this.listView = listView;
        this.noteIndex = noteIndex;
        this.target = target;
        this.previousStates = new List<(int, string)>();

        IEnumerable<ListViewItem> items;
        if (target == Target.Checked)
        {
            items = listView.CheckedItems.Cast<ListViewItem>();
        }
        else if (target == Target.Both)
        {
            // A item can be both checked and selected, so use Distinct to avoid duplicates
            items = listView.Items.Cast<ListViewItem>().Where(i => i.Checked || i.Selected).Distinct();
        }
        else
        {
            items = listView.SelectedItems.Cast<ListViewItem>();
        }

        foreach (var item in items)
        {
            int idx = item.Index;
            string prev = (item.SubItems.Count > noteIndex) ? item.SubItems[noteIndex].Text : string.Empty;
            previousStates.Add((idx, prev));
        }
    }

    public void Execute()
    {
        foreach (var (Index, _) in previousStates)
        {
            if (Index >= 0 && Index < listView.Items.Count)
            {
                var item = listView.Items[Index];
                while (item.SubItems.Count <= noteIndex)
                {
                    item.SubItems.Add(string.Empty);
                }
                item.SubItems[noteIndex].Text = string.Empty;
            }
        }
    }

    public void Undo()
    {
        foreach (var (Index, PreviousText) in previousStates)
        {
            if (Index >= 0 && Index < listView.Items.Count)
            {
                var item = listView.Items[Index];
                while (item.SubItems.Count <= noteIndex)
                {
                    item.SubItems.Add(string.Empty);
                }
                item.SubItems[noteIndex].Text = PreviousText;
            }
        }
    }
}