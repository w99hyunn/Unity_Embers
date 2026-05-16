using UnityEngine;

namespace Embers
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