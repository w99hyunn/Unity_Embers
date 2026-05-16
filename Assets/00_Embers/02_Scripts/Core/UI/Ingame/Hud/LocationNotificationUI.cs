using Michsky.UI.Reach;
using UnityEngine;

namespace Embers
{
    public class LocationNotificationUI : MonoBehaviour
    {
        [Header("Location Alert")]
        [SerializeField] private FeedNotification localtionNoti;
        [SerializeField] private HudUIController hudUIController;

        public void LocationNoti(string message)
        {
            localtionNoti.notificationText = message;
            localtionNoti.ExpandNotification();
            hudUIController.MapNameChange(message);
        }
    }
}