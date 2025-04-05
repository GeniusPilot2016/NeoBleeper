using System.ComponentModel;
using System.Diagnostics;

public class RewindCommand : ICommand
{
    private readonly Originator originator;
    private readonly Memento targetMemento;
    private readonly Memento currentMemento;

    // Kaydedilen durum deðerlerini saklamak için
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
        // Önce ListView durumunu geri yükle
        originator.SetMemento(targetMemento);

        // Sonra kaydedilen diðer UI deðerlerini ve deðiþkenleri geri yükle
        RestoreState(savedState);

        Debug.WriteLine("RewindCommand: Restored to saved state");
    }

    public void Undo()
    {
        // ListView durumunu mevcut duruma geri yükle
        originator.SetMemento(currentMemento);

        // Sonra mevcut diðer UI deðerlerini ve deðiþkenleri geri yükle
        RestoreState(currentState);

        Debug.WriteLine("RewindCommand: Undone - restored to current state");
    }

    // Kaydedilen durumu oluþtur
    private Dictionary<string, object> GetSavedState()
    {
        var state = new Dictionary<string, object>();

        // main_window örneðine eriþim saðlayalým
        var mainForm = Application.OpenForms.OfType<NeoBleeper.main_window>().FirstOrDefault();
        if (mainForm != null)
        {
            try
            {
                // Bu deðerleri dosyadan/kayýttan alma
                // Örnek: state["bpm"] = savedBpmValue; 
                // Þimdilik varsayýlan deðerleri kullanalým
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

    // Mevcut durumu oluþtur
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

    // Durumu geri yükle
    private void RestoreState(Dictionary<string, object> state)
    {
        var mainForm = Application.OpenForms.OfType<NeoBleeper.main_window>().FirstOrDefault();
        if (mainForm != null)
        {
            try
            {
                // BPM deðerini geri yükle
                if (state.TryGetValue("bpm", out object bpmObj) && bpmObj is int bpm)
                {
                    NeoBleeper.main_window.Variables.bpm = bpm;

                    // UI'ý güncelle
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

                // Alternating note length deðerini geri yükle
                if (state.TryGetValue("alternating_note_length", out object anlObj) && anlObj is int anl)
                {
                    NeoBleeper.main_window.Variables.alternating_note_length = anl;

                    // UI'ý güncelle
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
