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

using System.Xml;
using System.Xml.Serialization;
public class NBPMLFile
{
    [XmlRoot("NeoBleeperProjectFile")]
    public class NeoBleeperProjectFile
    {
        public Settings Settings { get; set; }
        public List LineList { get; set; }
    }

    public class Settings
    {
        public RandomSettings RandomSettings { get; set; }
        public PlaybackSettings PlaybackSettings { get; set; }
        public ClickPlayNotes ClickPlayNotes { get; set; }
        public PlayNotes PlayNotes { get; set; }
    }

    public class RandomSettings
    {
        public string KeyboardOctave { get; set; }
        public string BPM { get; set; }
        public string TimeSignature { get; set; }
        public string NoteSilenceRatio { get; set; }
        public string NoteLength { get; set; }
        public string AlternateTime { get; set; }
    }

    public class PlaybackSettings
    {
        public string NoteClickPlay { get; set; }
        public string NoteClickAdd { get; set; }
        public string AddNote1 { get; set; }
        public string AddNote2 { get; set; }
        public string AddNote3 { get; set; }
        public string AddNote4 { get; set; }
        public string NoteReplace { get; set; }
        public string NoteLengthReplace { get; set; }
    }

    public class ClickPlayNotes
    {
        public string ClickPlayNote1 { get; set; }
        public string ClickPlayNote2 { get; set; }
        public string ClickPlayNote3 { get; set; }
        public string ClickPlayNote4 { get; set; }
    }

    public class PlayNotes
    {
        public string PlayNote1 { get; set; }
        public string PlayNote2 { get; set; }
        public string PlayNote3 { get; set; }
        public string PlayNote4 { get; set; }
    }

    public class List
    {
        [XmlElement("Line")]
        public Line[] Lines { get; set; }
    }

    public class Line
    {
        public string Length { get; set; }
        public string Note1 { get; set; }
        public string Note2 { get; set; }
        public string Note3 { get; set; }
        public string Note4 { get; set; }
        public string Mod { get; set; }
        public string Art { get; set; }
    }
}