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
    /// <summary>
    /// Determines whether two specified ListViewItem instances are equal by comparing the text of their subitems.
    /// </summary>
    /// <remarks>The comparison is case-sensitive and considers the order and count of subitems. If either
    /// parameter is null, the method returns false.</remarks>
    /// <param name="x">The first ListViewItem to compare. Can be null.</param>
    /// <param name="y">The second ListViewItem to compare. Can be null.</param>
    /// <returns>true if both ListViewItem instances have the same number of subitems and the text of each corresponding subitem
    /// is equal; otherwise, false.</returns>
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

    /// <summary>
    /// Returns a hash code for the specified ListViewItem.
    /// </summary>
    /// <param name="obj">The ListViewItem for which to retrieve the hash code.</param>
    /// <returns>A 32-bit signed integer hash code for the specified ListViewItem.</returns>
    public int GetHashCode(ListViewItem obj)
    {
        return obj.GetHashCode();
    }
}