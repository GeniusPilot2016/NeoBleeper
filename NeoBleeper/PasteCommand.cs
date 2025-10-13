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

public class PasteCommand : ICommand
{
    private ListView listView;
    private ListViewItem newItem;
    private int insertIndex;

    public PasteCommand(ListView listView, ListViewItem newItem, int insertIndex)
    {
        this.listView = listView;
        this.newItem = newItem;
        this.insertIndex = insertIndex;
    }

    public void Execute()
    {
        if (insertIndex >= 0 && insertIndex < listView.Items.Count)
        {
            listView.Items.Insert(insertIndex, newItem);
        }
        else
        {
            listView.Items.Add(newItem);
        }
    }

    public async void Undo()
    {
        listView.Items.Remove(newItem);
    }
}
