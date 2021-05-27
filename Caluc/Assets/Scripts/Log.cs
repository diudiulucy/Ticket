using UnityEngine;
public class Log
{
    /*
        普通的Log
     */
    public static void i(string message)
    {
        if (!GlobalConfig.isPrintDebug)
        {
            return;
        }

        Debug.Log(message);
    }

    public static void e(string message)
    {
        if (!GlobalConfig.isPrintDebug)
        {
            return;
        }
        Debug.LogError(message);
    }
}