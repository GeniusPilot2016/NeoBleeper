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