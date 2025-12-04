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
