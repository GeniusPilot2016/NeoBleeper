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

public class ReplaceNoteCommand : ICommand
{
    private ListView listView;
    private int noteIndex;
    private string newNote;
    private List<string> previousNotes;

    public ReplaceNoteCommand(ListView listView, int noteIndex, string newNote)
    {
        this.listView = listView;
        this.noteIndex = noteIndex;
        this.newNote = newNote;
        this.previousNotes = listView.SelectedItems.Cast<ListViewItem>().Select(item => item.SubItems[noteIndex].Text).ToList();
    }

    public void Execute()
    {
        foreach (ListViewItem item in listView.SelectedItems)
        {
            item.SubItems[noteIndex].Text = newNote;
        }
    }

    public void Undo()
    {
        for (int i = 0; i < listView.SelectedItems.Count; i++)
        {
            listView.SelectedItems[i].SubItems[noteIndex].Text = previousNotes[i];
        }
    }
}
