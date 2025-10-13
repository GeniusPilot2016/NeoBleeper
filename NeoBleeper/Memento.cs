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

    public override bool Equals(object obj)
    {
        if (obj is Memento other)
        {
            return Items.SequenceEqual(other.Items, new ListViewItemComparer());
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Items.GetHashCode();
    }
}