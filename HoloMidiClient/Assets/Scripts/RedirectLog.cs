using TMPro;
using UnityEngine;

public class RedirectLog : MonoBehaviour
{
    public TextMeshPro DebugText;

    public void Start()
    {
        Application.logMessageReceived += Application_logMessageReceived;
    }

    private void Application_logMessageReceived(string logString, string stackTrace, LogType type)
    {
        if (LogFilter(logString))
            DebugText.text += (logString + "\n");
    }

    private static bool LogFilter(string logString)
    {
        if (logString.Contains("HandConstraintPalmUp") ||
            logString.Contains("proximity light")) 
            return false;

        return true;
    }
}
