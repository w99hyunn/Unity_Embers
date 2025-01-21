using Mirror;
using UnityEngine;

namespace STARTING
{
    public class HudUIController : MonoBehaviour
    {
        public HudUIView _view;

        public void MapNameChange(string mapName)
        {
            _view.mapName.SetText(mapName);
        }
    }
}