using UnityEngine;

namespace Embers
{
    public class OpenWebsite : MonoBehaviour
    {
        public void Open(string site)
        {
            Application.OpenURL(site);
        }
    }
}