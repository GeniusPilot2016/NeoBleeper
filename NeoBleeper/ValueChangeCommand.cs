using System.ComponentModel;
using System.Diagnostics;

public class ValueChangeCommand : ICommand
{
    private readonly string propertyName;
    private readonly int oldValue;
    private readonly int newValue;
    private readonly NumericUpDown control;
    private readonly bool isVariablesBpm;
    private readonly Action<int> updateVariable;

    public ValueChangeCommand(string propertyName, int oldValue, int newValue, NumericUpDown control, bool isVariablesBpm)
    {
        this.propertyName = propertyName;
        this.oldValue = oldValue;
        this.newValue = newValue;
        this.control = control;
        this.isVariablesBpm = isVariablesBpm;

        // Do�ru de�i�keni g�ncelleyen lambda tan�mla
        updateVariable = isVariablesBpm
            ? (value => NeoBleeper.main_window.Variables.bpm = value)
            : (value => NeoBleeper.main_window.Variables.alternating_note_length = value);
    }

    public void Execute()
    {
        SetValue(newValue);
    }

    public void Undo()
    {
        SetValue(oldValue);
    }

    private void SetValue(int value)
    {
        try
        {
            // Her zaman �nce de�i�keni g�ncelle
            updateVariable(value);

            // Sonra UI'� g�ncelle (gerekirse farkl� thread'de)
            if (control != null && !control.IsDisposed)
            {
                if (control.InvokeRequired)
                {
                    control.BeginInvoke(new Action(() =>
                    {
                        // ValueChanged olay�n� ge�ici olarak devre d��� b�rakmak i�in bir bayrak kullan
                        control.Tag = "SkipValueChanged";
                        control.Value = value;
                        control.Tag = null;
                    }));
                }
                else
                {
                    control.Tag = "SkipValueChanged";
                    control.Value = value;
                    control.Tag = null;
                }
            }

            Debug.WriteLine($"ValueChangeCommand: {propertyName} set to {value}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ValueChangeCommand.SetValue failed: {ex.Message}");
        }
    }
}
