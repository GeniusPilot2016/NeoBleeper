// NeoBleeper - AI-enabl// NeoBleeper - AI-enabled tune creation software using the system speaker (aka PC Speaker) on the motherboard
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