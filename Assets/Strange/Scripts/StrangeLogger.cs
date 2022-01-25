using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrangeLogger
{
    public static string StrangeErrorPrefix = "STRANGE ERROR:: ";
    public static string StrangeLogPrefix = "STRANGE LOG:: ";

    public static void Log(string logInfo)
    {
        Debug.Log(StrangeLogPrefix + logInfo);
    }
    public static void LogWarning(string logInfo)
    {
        Debug.LogWarning(StrangeErrorPrefix + logInfo);
    }
    public static void LogError(string logInfo)
    {
        UnityEditor.EditorUtility.DisplayDialog(StrangeErrorPrefix, logInfo, "okay");
    }
}
