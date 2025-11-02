using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.Reach
{
    public class ChapterIdentifier : MonoBehaviour
    {
        [Header("Resources")]
        public Animator animator;
        [SerializeField] private RectTransform backgroundRect;
        public Image backgroundImage;
        public TextMeshProUGUI titleObject;
        public TextMeshProUGUI classNameObject;
        public TextMeshProUGUI descriptionObject;
        public ButtonManager deleteButton;
        public ButtonManager createButton;
        public ButtonManager playButton;
        public GameObject completedIndicator;
        public GameObject unlockedIndicator;
        public GameObject lockedIndicator;

        [HideInInspector] public ChapterManager chapterManager;
        [HideInInspector] public bool isLocked;
        [HideInInspector] public bool isCurrent;

        public void UpdateBackgroundRect() 
        { 
            chapterManager.currentBackgroundRect = backgroundRect;
            chapterManager.DoStretch();
        }

        public void SetCharacterPlayAndDelete()
        {
            //completedIndicator.SetActive(false);
            //unlockedIndicator.SetActive(true);
            //lockedIndicator.SetActive(false);

            deleteButton.gameObject.SetActive(true);
            createButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(true);

            isLocked = false;
            isCurrent = true;
            deleteButton.isInteractable = true;
            playButton.isInteractable = true;
        }

        public void SetLocked()
        {
            //completedIndicator.SetActive(false);
            //unlockedIndicator.SetActive(false);
            //lockedIndicator.SetActive(true);

            deleteButton.gameObject.SetActive(false);
            createButton.gameObject.SetActive(true);
            playButton.gameObject.SetActive(false);

            isLocked = true;
            isCurrent = false;
            createButton.isInteractable = false;
        }

        public void SetCreate()
        {
            //completedIndicator.SetActive(false);
            //unlockedIndicator.SetActive(true);
            //lockedIndicator.SetActive(false);

            deleteButton.gameObject.SetActive(false);
            createButton.gameObject.SetActive(true);
            playButton.gameObject.SetActive(false);

            isLocked = false;
            isCurrent = false;
            createButton.isInteractable = true;
        }

        public void SetCharacterPlay()
        {
            //completedIndicator.SetActive(true);
            //unlockedIndicator.SetActive(false);
            //lockedIndicator.SetActive(false);

            deleteButton.gameObject.SetActive(false);
            createButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(true);

            isLocked = false;
            isCurrent = false;
            playButton.isInteractable = true;
        }
    }
}