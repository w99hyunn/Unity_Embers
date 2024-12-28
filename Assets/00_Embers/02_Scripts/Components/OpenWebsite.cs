using UnityEngine;

namespace STARTING
{
    public class OpenWebsite : MonoBehaviour
    {
        public void Open(string site)
        {
            Application.OpenURL(site);
        }
    }
}