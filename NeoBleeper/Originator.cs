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

    public SavedStateMemento CreateSavedStateMemento(int bpmValue, int alternatingNoteLength, 
        double noteSilenceRatio, int timeSignature)
    {
        var items = listView.Items.Cast<ListViewItem>().Select(item => (ListViewItem)item.Clone()).ToList();
        return new SavedStateMemento(items, bpmValue, alternatingNoteLength, noteSilenceRatio, timeSignature);
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
