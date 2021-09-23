using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrangeLogger : MonoBehaviour
{
    // StrangeLogger is just a little add-on i made so that players can toggle visibility of
    // these logs by simply unticking a checkbox in the unity inspector
    // eventually this will log to a file so that anyone having any issues with Strange can
    // send in logs and any issues can be spotted in the logs without the need to transfer whole projects back and forth


    public static string StrangeErrorPrefix = "STRANGE ERROR:: ";
    public static string StrangeLogPrefix = "STRANGE LOG:: ";
    public static StrangeLogger singleton;
    [Tooltip("when true, all actions in Strange will be logged directly to the console")]
    public bool showInConsole = false;
    [Tooltip("when true, StrangeErrors will be prioritised and trigger an errorPause ")]
    public bool priorityErrors = false;
    // Start is called before the first frame update
    void Start()
    {
        singleton = this;
    }

    public static void Log(string logInfo)
    {
        if(singleton.showInConsole)
        {
            Debug.Log(StrangeLogPrefix + logInfo);
        }
    }
    public static void LogError(string logInfo)
    {
        if (singleton.showInConsole)
        {
            if (singleton.priorityErrors)
                Debug.LogError(StrangeErrorPrefix + logInfo);
            else
                Debug.LogWarning(StrangeErrorPrefix + logInfo);
        }

    }
}
