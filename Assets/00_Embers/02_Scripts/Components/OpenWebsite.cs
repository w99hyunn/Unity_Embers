using UnityEngine;

namespace NOLDA
{
    public class OpenWebsite : MonoBehaviour
    {
        public void Open(string site)
        {
            Application.OpenURL(site);
        }
    }
}