using NeoBleeper;
using System.Diagnostics;

public class SavedStateMemento : Memento
{
    public int BpmValue { get; }
    public int AlternatingNoteLength { get; }

    public SavedStateMemento(List<ListViewItem> items, int bpmValue, int alternatingNoteLength)
        : base(items)
    {
        BpmValue = bpmValue;
        AlternatingNoteLength = alternatingNoteLength;
    }
}

public class RewindCommand : ICommand
{
    private readonly Originator originator;
    private readonly Memento savedState;
    private Memento currentState;
    private readonly main_window mainWindow;

    public RewindCommand(Originator originator, Memento savedState, main_window mainWindow = null)
    {
        if (originator == null)
            throw new ArgumentNullException(nameof(originator));

        if (savedState == null)
            throw new ArgumentNullException(nameof(savedState));

        this.originator = originator;
        this.savedState = savedState;
        this.currentState = originator.CreateMemento();
        this.mainWindow = mainWindow ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] as main_window : null);
    }

    public void Execute()
    {
        // Save current state
        currentState = originator.CreateMemento();

        try
        {
            // Turn back into saved state
            originator.SetMemento(savedState);

            // Reload the BPM and alternating note length values
            if (savedState is SavedStateMemento state && mainWindow != null)
            {
                mainWindow.RestoreVariableValues(state.BpmValue, state.AlternatingNoteLength);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in RewindCommand.Execute: {ex.Message}");
            throw;
        }
    }

    public void Undo()
    {
        try
        {
            // Turn back into current state
            originator.SetMemento(currentState);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in RewindCommand.Undo: {ex.Message}");
            throw;
        }
    }
}
