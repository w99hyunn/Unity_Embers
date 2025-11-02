using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Michsky.UI.Reach
{
    public class Logout : MonoBehaviour
    {
        public void LogoutGame()
        {
            Destroy(GameObject.Find("[Reach UI - Splash Screen Helper]"));
            NetworkClient.Disconnect();
            SceneManager.LoadScene("Title");
            SceneManager.LoadSceneAsync("Session", LoadSceneMode.Additive);
        }
    }
}