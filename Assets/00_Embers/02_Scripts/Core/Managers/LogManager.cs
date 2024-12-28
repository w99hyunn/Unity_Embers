using UnityEngine;

namespace STARTING
{
    public class LogManager : MonoBehaviour
    {
        public void Log(string message)
        {
#if UNITY_SERVER || UNITY_EDITOR
            Debug.Log(message);
#endif
        }

        public void LogWarning(string message)
        {
#if UNITY_SERVER || UNITY_EDITOR
            Debug.LogWarning(message);
#endif
        }

        public void LogError(string message)
        {
#if UNITY_SERVER || UNITY_EDITOR
            Debug.LogError(message);
#endif
        }
    }
}