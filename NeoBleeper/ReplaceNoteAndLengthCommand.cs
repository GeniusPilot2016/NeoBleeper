public class ReplaceNoteAndLengthCommand : ICommand
{
    private ListView listView;
    private int noteIndex;
    private string newLength;
    private string newNote;
    private List<(string PreviousNote, string PreviousLength)> previousValues;

    public ReplaceNoteAndLengthCommand(ListView listView, string newLength, int noteIndex, string newNote)
    {
        this.listView = listView;
        this.noteIndex = noteIndex;
        this.newLength = newLength;
        this.newNote = newNote;

        // Store both previous note and length for undo functionality
        this.previousValues = listView.SelectedItems.Cast<ListViewItem>()
            .Select(item => (
                PreviousNote: item.SubItems[noteIndex].Text,
                PreviousLength: item.SubItems[0].Text
            )).ToList();
    }

    public void Execute()
    {
        foreach (ListViewItem item in listView.SelectedItems)
        {
            item.SubItems[0].Text = newLength; // Update length
            item.SubItems[noteIndex].Text = newNote; // Update note
        }
    }

    public void Undo()
    {
        for (int i = 0; i < listView.SelectedItems.Count; i++)
        {
            listView.SelectedItems[i].SubItems[0].Text = previousValues[i].PreviousLength; // Restore length
            listView.SelectedItems[i].SubItems[noteIndex].Text = previousValues[i].PreviousNote; // Restore note
        }
    }
}