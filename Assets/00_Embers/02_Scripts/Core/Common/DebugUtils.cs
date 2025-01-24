using UnityEngine;

namespace NOLDA
{
    public static class DebugUtils
    {
        [System.Diagnostics.Conditional("UNITY_SERVER")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Log(object message)
        {
            Debug.Log(message);
        }

        [System.Diagnostics.Conditional("UNITY_SERVER")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }

        [System.Diagnostics.Conditional("UNITY_SERVER")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogError(object message)
        {
            Debug.LogError(message);
        }
    }
}