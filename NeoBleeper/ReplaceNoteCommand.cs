using System.ComponentModel;

public class ReplaceNoteCommand : ICommand
{
    private ListView listView;
    private int noteIndex;
    private string newNote;
    private List<string> previousNotes;

    public ReplaceNoteCommand(ListView listView, int noteIndex, string newNote)
    {
        this.listView = listView;
        this.noteIndex = noteIndex;
        this.newNote = newNote;
        this.previousNotes = listView.SelectedItems.Cast<ListViewItem>().Select(item => item.SubItems[noteIndex].Text).ToList();
    }

    public async void Execute()
    {
        try
        {
            foreach (ListViewItem item in listView.SelectedItems)
            {
                item.SubItems[noteIndex].Text = newNote;
            }
        }
        catch (InvalidAsynchronousStateException)
        {
            return;
        }
    }

    public async void Undo()
    {
        try
        {
            for (int i = 0; i < listView.SelectedItems.Count; i++)
            {
                listView.SelectedItems[i].SubItems[noteIndex].Text = previousNotes[i];
            }
        }
        catch (InvalidAsynchronousStateException)
        {
            return;
        }
    }
}
