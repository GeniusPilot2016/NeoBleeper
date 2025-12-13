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
using NeoBleeper.Properties;
using System.ComponentModel;

public class ValueChangeCommand : ICommand
{
    private readonly string variableName;
    private readonly int oldValue;
    private readonly int newValue;
    private readonly double oldDoubleValue;
    private readonly double newDoubleValue;
    private readonly NumericUpDown numericUpDown;
    private readonly TrackBar trackBar;
    private readonly bool isBpm;
    private readonly Type type;
    private readonly Label label;
    private readonly bool isDouble;
    private readonly bool isNoteSilenceRatio;
    enum Type
    {
        NumericUpDown,
        TrackBar
    }

    public ValueChangeCommand(string variableName, int oldValue, int newValue, NumericUpDown numericUpDown, bool isBpm)
    {
        isDouble = false;
        this.variableName = variableName;
        this.oldValue = oldValue;
        this.newValue = newValue;
        this.numericUpDown = numericUpDown;
        this.isBpm = isBpm;
        type = Type.NumericUpDown;
    }
    public ValueChangeCommand(string variableName, int oldValue, int newValue, TrackBar trackBar, Label label = null)
    {
        isDouble = false;
        this.variableName = variableName;
        this.oldValue = oldValue;
        this.newValue = newValue;
        this.trackBar = trackBar;
        type = Type.TrackBar;
        this.label = label;
    }
    public ValueChangeCommand(string variableName, double oldDoubleValue, double newDoubleValue, TrackBar trackBar, bool isNoteSilenceRatio, Label label = null)
    {
        isDouble = true;
        this.isNoteSilenceRatio = isNoteSilenceRatio;
        this.variableName = variableName;
        this.oldDoubleValue = oldDoubleValue;
        this.newDoubleValue = newDoubleValue;
        this.trackBar = trackBar;
        type = Type.TrackBar;
        this.label = label;
    }

    /// <summary>
    /// Executes the update operation using the current value type.
    /// </summary>
    /// <remarks>This method selects the appropriate value to update based on whether the double value is
    /// active. It should be called when the value needs to be refreshed or applied.</remarks>
    public void Execute()
    {
        if (isDouble)
        {
            UpdateValue(newDoubleValue);
        }
        else
        {
            UpdateValue(newValue);
        }
    }

    /// <summary>
    /// Reverts the most recent change to the value, restoring it to its previous state.
    /// </summary>
    /// <remarks>Use this method to undo the last modification made to the value. This operation is typically
    /// used to support undo functionality in user interfaces or data editing scenarios. If no changes have been made
    /// since the last undo, calling this method has no effect.</remarks>
    public void Undo()
    {
        if (isDouble)
        {
            UpdateValue(oldDoubleValue);
        }
        else
        {
            UpdateValue(oldValue);
        }
    }

    /// <summary>
    /// Updates the current value of the control based on the specified input.
    /// </summary>
    /// <param name="value">The new value to set for the control.</param>
    /// <exception cref="InvalidEnumArgumentException">Thrown if the control type is not recognized.</exception>
    private void UpdateValue(int value)
    {
        switch (type)
        {
            case Type.NumericUpDown:
                UpdateNumericUpDown(value);
                break;
            case Type.TrackBar:
                UpdateTrackBar(value);
                break;
            default:
                throw new InvalidEnumArgumentException();
        }

    }

    /// <summary>
    /// Updates the current value based on the specified input and the configured type.
    /// </summary>
    /// <param name="value">The new value to apply. The interpretation of this value depends on the current type configuration.</param>
    /// <exception cref="InvalidEnumArgumentException">Thrown if the current type is not a supported value.</exception>
    private void UpdateValue(double value)
    {
        switch (type)
        {
            case Type.TrackBar:
                UpdateTrackBar(value, isNoteSilenceRatio);
                break;
            default:
                throw new InvalidEnumArgumentException();
        }

    }

    /// <summary>
    /// Updates the value of the numeric up-down control and synchronizes the corresponding variable based on the
    /// current mode.
    /// </summary>
    /// <remarks>If the control is in BPM mode, this method updates the BPM variable; otherwise, it updates
    /// the alternating note length variable. The method temporarily sets a tag on the control to prevent recursive
    /// value change events during the update.</remarks>
    /// <param name="value">The new value to set for the numeric up-down control and the associated variable.</param>
    private void UpdateNumericUpDown(int value)
    {
        // Usage of flag to prevent recursion
        numericUpDown.Tag = "SkipValueChanged";
        numericUpDown.Value = value;

        if (isBpm)
        {
            MainWindow.Variables.bpm = value;
        }
        else
        {
            MainWindow.Variables.alternatingNoteLength = value;
        }

        numericUpDown.Tag = null;
    }

    /// <summary>
    /// Synchronizes the track bar control, associated label, and time signature variable with the specified value.
    /// </summary>
    /// <remarks>This method temporarily sets a tag on the track bar to prevent recursive value change events
    /// while updating its value. If a label is associated, its text is also updated to reflect the new value.</remarks>
    /// <param name="value">The new value to assign to the track bar, label, and time signature variable.</param>
    private void UpdateTrackBar(int value)
    {
        // Usage of flag to prevent recursion
        trackBar.Tag = "SkipValueChanged";
        trackBar.Value = value;
        MainWindow.Variables.timeSignature = value;
        if (label != null)
        {
            label.Text = value.ToString();
        }
        trackBar.Tag = null;
    }

    /// <summary>
    /// Updates the track bar and associated label to reflect the specified value, optionally formatting the label as a
    /// percentage for note silence ratio settings.
    /// </summary>
    /// <remarks>This method temporarily sets the track bar's Tag property to prevent recursive value change
    /// events while updating the control. The label is updated only if it is not null.</remarks>
    /// <param name="value">The new value to set on the track bar. Typically expected to be between 0.0 and 1.0, representing a ratio or
    /// percentage.</param>
    /// <param name="isNoteSilenceRatio">true to format the label as a percentage for note silence ratio; otherwise, false to display the raw value.</param>
    private void UpdateTrackBar(double value, bool isNoteSilenceRatio)
    {
        // Usage of flag to prevent recursion
        trackBar.Tag = "SkipValueChanged";
        trackBar.Value = (int)(value * 100);
        MainWindow.Variables.noteSilenceRatio = value;
        int percent = (int)(value * 100);
        if (label != null)
        {
            if (isNoteSilenceRatio)
            {
                label.Text = Resources.TextPercent.Replace("{number}", percent.ToString());
            }
            else
            {
                label.Text = value.ToString();
            }
        }
        trackBar.Tag = null;
    }
}
