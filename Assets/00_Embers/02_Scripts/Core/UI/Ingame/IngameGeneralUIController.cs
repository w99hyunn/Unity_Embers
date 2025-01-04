using UnityEngine;
using UnityEngine.SceneManagement;

namespace STARTING
{
    public class IngameGeneralUIController : MonoBehaviour
    {
        public IngameUI ingameUI;
        
        public void ReturnTitle()
        {
            ingameUI.localPlayer.GetComponent<Player>().CmdRemovePlayer();
            Managers.Map.ReturnTitle();
        }
    }
}