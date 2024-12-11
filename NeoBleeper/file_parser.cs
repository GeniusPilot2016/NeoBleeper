using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

namespace NeoBleeper
{
    public class file_parser
    {
        public class NeoBleeperProjectMarkupLanguageFileParser
        {
            public int KeyboardOctave { get; private set; }
            public int BPM { get; private set; }
            public int TimeSig { get; private set; }
            public int NoteSilenceRatio { get; private set; }
            public int NoteLength { get; private set; }
            public int AlternateTime { get; private set; }
            public bool NoteClickPlay { get; private set; }
            public bool NoteClickAdd { get; private set; }
            public bool AddNote1 { get; private set; }
            public bool AddNote2 { get; private set; }
            public bool AddNote3 { get; private set; }
            public bool AddNote4 { get; private set; }
            public bool NoteReplace { get; private set; }
            public int NoteLengthReplace { get; private set; }
            public bool ClickPlayNote1 { get; private set; }
            public bool ClickPlayNote2 { get; private set; }
            public bool ClickPlayNote3 { get; private set; }
            public bool ClickPlayNote4 { get; private set; }
            public bool PlayNote1 { get; private set; }
            public bool PlayNote2 { get; private set; }
            public bool PlayNote3 { get; private set; }
            public bool PlayNote4 { get; private set; }
            public List<string> MusicList { get; private set; } = new List<string>();
        }
        public class BleeperMusicMakerFileParser
        {
            public int KeyboardOctave { get; private set; }
            public int BPM { get; private set; }
            public int TimeSig { get; private set; }
            public int NoteSilenceRatio { get; private set; }
            public int NoteLength { get; private set; }
            public int AlternateTime { get; private set; }
            public bool NoteClickPlay { get; private set; }
            public bool NoteClickAdd { get; private set; }
            public bool AddNote1 { get; private set; }
            public bool AddNote2 { get; private set; }
            public bool NoteReplace { get; private set; }
            public int NoteLengthReplace { get; private set; }
            public bool ClickPlayNote1 { get; private set; }
            public bool ClickPlayNote2 { get; private set; }
            public bool PlayNote1 { get; private set; }
            public bool PlayNote2 { get; private set; }
            public List<string> MusicList { get; private set; } = new List<string>();

            public void Parse(string filePath)
            {
                var lines = File.ReadAllLines(filePath); bool musicListStarted = false; foreach (var line in lines)
                {
                    if (line.StartsWith("//") || string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    if (line == "MUSICLISTSTART") 
                    { 
                        musicListStarted = true; 
                        continue; 
                    }
                    if (musicListStarted) { MusicList.Add(line); }
                    else
                    {
                        var parts = line.Split(' '); switch (parts[0])
                        {
                            case "KeyboardOctave": KeyboardOctave = int.Parse(parts[1]); break;
                            case "BPM": BPM = int.Parse(parts[1]); break;
                            case "TimeSig": TimeSig = int.Parse(parts[1]); break;
                            case "NoteSilenceRatio": NoteSilenceRatio = int.Parse(parts[1]); break;
                            case "NoteLength": NoteLength = int.Parse(parts[1]); break;
                            case "AlternateTime": AlternateTime = int.Parse(parts[1]); break;
                            case "NoteClickPlay": NoteClickPlay = parts[1] == "1"; break;
                            case "NoteClickAdd": NoteClickAdd = parts[1] == "1"; break;
                            case "AddNote1": AddNote1 = parts[1] == "True"; break;
                            case "AddNote2": AddNote2 = parts[1] == "False"; break;
                            case "NoteReplace": NoteReplace = parts[1] == "1"; break;
                            case "NoteLengthReplace": NoteLengthReplace = int.Parse(parts[1]); break;
                            case "ClickPlayNote1": ClickPlayNote1 = parts[1] == "1"; break;
                            case "ClickPlayNote2": ClickPlayNote2 = parts[1] == "1"; break;
                            case "PlayNote1": PlayNote1 = parts[1] == "1"; break;
                            case "PlayNote2": PlayNote2 = parts[1] == "1"; break;
                            default: // Handle unknown settings if necessary
                                break;
                        }
                    }
                }
            }
        }
    }
}
