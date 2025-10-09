using System.ComponentModel;
using System.Diagnostics;
using NeoBleeper;
using NeoBleeper.Properties;

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
        if(isDouble)
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
            main_window.Variables.bpm = value;
        }
        else
        {
            main_window.Variables.alternating_note_length = value;
        }

        numericUpDown.Tag = null;
    }
    private void UpdateTrackBar(int value)
    {
        // Usage of flag to prevent recursion
        trackBar.Tag = "SkipValueChanged";
        trackBar.Value = value;
        main_window.Variables.time_signature = value;
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
        main_window.Variables.note_silence_ratio = value;
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
