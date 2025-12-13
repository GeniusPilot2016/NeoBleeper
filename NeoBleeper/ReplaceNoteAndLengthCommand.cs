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

    /// <summary>
    /// Updates the length and note values for all selected items in the list view.
    /// </summary>
    /// <remarks>This method modifies the text of specific subitems for each selected item. Ensure that the
    /// list view contains selected items and that the subitem indices used are valid to avoid runtime errors.</remarks>
    public void Execute()
    {
        foreach (ListViewItem item in listView.SelectedItems)
        {
            item.SubItems[0].Text = newLength; // Update length
            item.SubItems[noteIndex].Text = newNote; // Update note
        }
    }

    /// <summary>
    /// Reverts the changes made to the selected items in the list view, restoring their previous length and note
    /// values.
    /// </summary>
    /// <remarks>Use this method to undo the most recent modifications to the selected items. Only the length
    /// and note fields are restored to their previous states. This method has no effect if no items are
    /// selected.</remarks>
    public void Undo()
    {
        for (int i = 0; i < listView.SelectedItems.Count; i++)
        {
            listView.SelectedItems[i].SubItems[0].Text = previousValues[i].PreviousLength; // Restore length
            listView.SelectedItems[i].SubItems[noteIndex].Text = previousValues[i].PreviousNote; // Restore note
        }
    }
}