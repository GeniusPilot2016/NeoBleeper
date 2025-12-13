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

    /// <summary>
    /// Creates a memento that captures the current state of the list view items.
    /// </summary>
    /// <remarks>The returned memento contains copies of the list view items as they exist at the time of the
    /// call. Subsequent changes to the list view or its items are not reflected in the memento.</remarks>
    /// <returns>A <see cref="Memento"/> object containing a snapshot of the current list view items. The memento can be used to
    /// restore the list view to this state at a later time.</returns>
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

    /// <summary>
    /// Restores the state of the list view from the specified memento.
    /// </summary>
    /// <param name="memento">The memento containing the state to restore. Must not be null, and its Items collection must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="memento"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="memento"/>.Items is null.</exception>
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
