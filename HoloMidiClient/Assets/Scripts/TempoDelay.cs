using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TempoDelay : MonoBehaviour
{
    public int BeatsPerMeasure = 4;
    public int QuarterNoteBeats = 4;

    public int Tempo = 120;

    public float MeasuresToDelay = 1;
    public UnityEvent EventToDelay;

    public void FireEventDelayed()
    {
        StartCoroutine(DelayCoroutine());
    }

    public IEnumerator DelayCoroutine()
    {
        var delay = BeatsPerMeasure * QuarterNoteBeats / 4f * MeasuresToDelay * 60 / Tempo;
        yield return new WaitForSeconds(delay);

        EventToDelay.Invoke();
    }
}
