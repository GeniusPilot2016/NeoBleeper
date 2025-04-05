public class SavedStateMemento : Memento
{
    public int SavedBpm { get; }
    public int SavedAlternatingNoteLength { get; }

    public SavedStateMemento(List<ListViewItem> items, int savedBpm, int savedAlternatingNoteLength)
        : base(items)
    {
        SavedBpm = savedBpm;
        SavedAlternatingNoteLength = savedAlternatingNoteLength;
    }
}
