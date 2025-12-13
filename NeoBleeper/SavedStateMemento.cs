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

using NeoBleeper;

public class SavedStateMemento : Memento
{
    public int BpmValue { get; }
    public int AlternatingNoteLength { get; }
    public double NoteSilenceRatio { get; }
    public int TimeSignature { get; }

    public SavedStateMemento(List<ListViewItem> items, int bpmValue, int alternatingNoteLength,
        double noteSilenceRatio, int timeSignature)
        : base(items)
    {
        BpmValue = bpmValue;
        AlternatingNoteLength = alternatingNoteLength;
        NoteSilenceRatio = noteSilenceRatio;
        TimeSignature = timeSignature;
    }
}

public class RewindCommand : ICommand
{
    private readonly Originator originator;
    private readonly Memento savedState;
    private Memento currentState;
    private readonly MainWindow mainWindow;

    public RewindCommand(Originator originator, Memento savedState, MainWindow mainWindow = null)
    {
        if (originator == null)
            throw new ArgumentNullException(nameof(originator));

        if (savedState == null)
            throw new ArgumentNullException(nameof(savedState));

        this.originator = originator;
        this.savedState = savedState;
        this.currentState = originator.CreateMemento();
        this.mainWindow = mainWindow ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] as MainWindow : null);
    }

    /// <summary>
    /// Restores the application state to a previously saved state and reloads related musical parameters.
    /// </summary>
    /// <remarks>This method reverts the originator to the provided saved state and updates the main window's
    /// BPM, alternating note length, time signature, and note silence ratio values accordingly. If an error occurs
    /// during the operation, the exception is logged and rethrown.</remarks>
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
                mainWindow.RestoreVariableValues(state.BpmValue, state.AlternatingNoteLength,
                    state.TimeSignature, state.NoteSilenceRatio);
            }
        }
        catch (Exception ex)
        {
            Logger.Log($"Error in RewindCommand.Execute: {ex.Message}", Logger.LogTypes.Error);
            throw;
        }
    }

    /// <summary>
    /// Reverts the target object to its previous state.
    /// </summary>
    /// <remarks>Use this method to undo the most recent change applied to the object managed by this command.
    /// The ability to undo depends on the availability of a previously saved state. If no prior state exists, the
    /// object's state remains unchanged.</remarks>
    public void Undo()
    {
        try
        {
            // Turn back into current state
            originator.SetMemento(currentState);
        }
        catch (Exception ex)
        {
            Logger.Log($"Error in RewindCommand.Undo: {ex.Message}", Logger.LogTypes.Error);
            throw;
        }
    }
}
