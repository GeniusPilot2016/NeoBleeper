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

public class Memento
{
    public List<ListViewItem> Items { get; }

    public Memento(List<ListViewItem> items)
    {
        Items = items;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current Memento instance.
    /// </summary>
    /// <remarks>Equality is determined by comparing the Items collections of both Memento instances using a
    /// ListViewItemComparer. This method is intended to support value-based equality for Memento objects.</remarks>
    /// <param name="obj">The object to compare with the current Memento instance.</param>
    /// <returns>true if the specified object is a Memento and its items are equal to those of the current instance; otherwise,
    /// false.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Memento other)
        {
            return Items.SequenceEqual(other.Items, new ListViewItemComparer());
        }
        return false;
    }

    /// <summary>
    /// Serves as the default hash function for the object.
    /// </summary>
    /// <remarks>The hash code is based on the value of the Items property. Objects that are considered equal
    /// should return the same hash code.</remarks>
    /// <returns>A 32-bit signed integer hash code representing the current object.</returns>
    public override int GetHashCode()
    {
        return Items.GetHashCode();
    }
}