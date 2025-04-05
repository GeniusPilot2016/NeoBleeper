using System.ComponentModel;
using System.Diagnostics;

public class RewindCommand : ICommand
{
    private readonly Originator originator;
    private readonly Memento targetMemento;
    private readonly Memento currentMemento;

    // Kaydedilen durum de�erlerini saklamak i�in
    private readonly Dictionary<string, object> savedState;
    private readonly Dictionary<string, object> currentState;

    public RewindCommand(Originator originator, Memento targetMemento)
    {
        this.originator = originator;
        this.targetMemento = targetMemento;
        this.currentMemento = originator.CreateMemento();

        // Kaydedilen ve mevcut durumu kaydet
        savedState = GetSavedState();
        currentState = GetCurrentState();
    }

    public void Execute()
    {
        // �nce ListView durumunu geri y�kle
        originator.SetMemento(targetMemento);

        // Sonra kaydedilen di�er UI de�erlerini ve de�i�kenleri geri y�kle
        RestoreState(savedState);

        Debug.WriteLine("RewindCommand: Restored to saved state");
    }

    public void Undo()
    {
        // ListView durumunu mevcut duruma geri y�kle
        originator.SetMemento(currentMemento);

        // Sonra mevcut di�er UI de�erlerini ve de�i�kenleri geri y�kle
        RestoreState(currentState);

        Debug.WriteLine("RewindCommand: Undone - restored to current state");
    }

    // Kaydedilen durumu olu�tur
    private Dictionary<string, object> GetSavedState()
    {
        var state = new Dictionary<string, object>();

        // main_window �rne�ine eri�im sa�layal�m
        var mainForm = Application.OpenForms.OfType<NeoBleeper.main_window>().FirstOrDefault();
        if (mainForm != null)
        {
            try
            {
                // Bu de�erleri dosyadan/kay�ttan alma
                // �rnek: state["bpm"] = savedBpmValue; 
                // �imdilik varsay�lan de�erleri kullanal�m
                state["bpm"] = 140;
                state["alternating_note_length"] = 30;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting saved state: {ex.Message}");
            }
        }

        return state;
    }

    // Mevcut durumu olu�tur
    private Dictionary<string, object> GetCurrentState()
    {
        var state = new Dictionary<string, object>();

        try
        {
            state["bpm"] = NeoBleeper.main_window.Variables.bpm;
            state["alternating_note_length"] = NeoBleeper.main_window.Variables.alternating_note_length;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting current state: {ex.Message}");
        }

        return state;
    }

    // Durumu geri y�kle
    private void RestoreState(Dictionary<string, object> state)
    {
        var mainForm = Application.OpenForms.OfType<NeoBleeper.main_window>().FirstOrDefault();
        if (mainForm != null)
        {
            try
            {
                // BPM de�erini geri y�kle
                if (state.TryGetValue("bpm", out object bpmObj) && bpmObj is int bpm)
                {
                    NeoBleeper.main_window.Variables.bpm = bpm;

                    // UI'� g�ncelle
                    var bpmControl = mainForm.Controls.Find("numericUpDown_bpm", true).FirstOrDefault() as NumericUpDown;
                    if (bpmControl != null)
                    {
                        if (bpmControl.InvokeRequired)
                        {
                            bpmControl.Invoke(new Action(() =>
                            {
                                bpmControl.Value = bpm;
                            }));
                        }
                        else
                        {
                            bpmControl.Value = bpm;
                        }
                    }
                }

                // Alternating note length de�erini geri y�kle
                if (state.TryGetValue("alternating_note_length", out object anlObj) && anlObj is int anl)
                {
                    NeoBleeper.main_window.Variables.alternating_note_length = anl;

                    // UI'� g�ncelle
                    var anlControl = mainForm.Controls.Find("numericUpDown_alternating_notes", true).FirstOrDefault() as NumericUpDown;
                    if (anlControl != null)
                    {
                        if (anlControl.InvokeRequired)
                        {
                            anlControl.Invoke(new Action(() =>
                            {
                                anlControl.Value = anl;
                            }));
                        }
                        else
                        {
                            anlControl.Value = anl;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error restoring state: {ex.Message}");
            }
        }
    }
}
