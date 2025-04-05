using System.ComponentModel;
using System.Diagnostics;
using NeoBleeper;

public class ValueChangeCommand : ICommand
{
    private readonly string variableName;
    private readonly int oldValue;
    private readonly int newValue;
    private readonly NumericUpDown control;
    private readonly bool isBpm;

    public ValueChangeCommand(string variableName, int oldValue, int newValue, NumericUpDown control, bool isBpm)
    {
        this.variableName = variableName;
        this.oldValue = oldValue;
        this.newValue = newValue;
        this.control = control;
        this.isBpm = isBpm;
    }

    public void Execute()
    {
        UpdateValue(newValue);
    }

    public void Undo()
    {
        UpdateValue(oldValue);
    }

    private void UpdateValue(int value)
    {
        // De�er de�i�im olay�n�n gereksiz tetiklenmesini �nlemek i�in bayrak kullan�m�
        control.Tag = "SkipValueChanged";
        control.Value = value;

        if (isBpm)
        {
            main_window.Variables.bpm = value;
        }
        else
        {
            main_window.Variables.alternating_note_length = value;
        }

        control.Tag = null;
    }
}
