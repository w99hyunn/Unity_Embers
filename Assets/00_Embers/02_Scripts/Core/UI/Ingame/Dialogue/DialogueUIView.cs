using System.Collections.Generic;
using Michsky.UI.Reach;
using NOLDA;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ModalWindowManager))]
public class DialogueUIView : MonoBehaviour
{
    [SerializeField] private List<CanvasGroup> hideUIsOnDialogue;
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private TMP_Text npcRoleText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private ButtonManager nextButton;
    [SerializeField] private ButtonManager okButton;
    [SerializeField] private ButtonManager closeButton;

    private ModalWindowManager dialogueUI;
    public ModalWindowManager DialogueUI => dialogueUI;

    public string DialogueText
    {
        get => dialogueText.text;
        set
        {
            dialogueText.text = value;
        }
    }

    private void Awake()
    {
        TryGetComponent<ModalWindowManager>(out dialogueUI);
    }

    public void HideUI()
    {
        foreach (var canvasGroup in hideUIsOnDialogue)
        {
            FadeCanvasGroup(canvasGroup, false).Forget();
        }
    }

    public void ShowUI()
    {
        foreach (var canvasGroup in hideUIsOnDialogue)
        {
            FadeCanvasGroup(canvasGroup, true).Forget();
        }
    }

    private async Awaitable FadeCanvasGroup(CanvasGroup canvasGroup, bool fadeIn)
    {
        float duration = 0.5f; // Fade duration
        float elapsedTime = 0f;
        float startAlpha = fadeIn ? 0 : 1;
        float endAlpha = fadeIn ? 1 : 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            await Awaitable.NextFrameAsync();
        }

        canvasGroup.alpha = endAlpha;

        if (!fadeIn)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public event UnityAction NextButtonAction
    {
        add => nextButton.onClick.AddListener(value);
        remove => nextButton.onClick.RemoveListener(value);
    }

    public event UnityAction OkButtonAction
    {
        add => okButton.onClick.AddListener(value);
        remove => okButton.onClick.RemoveListener(value);
    }

    public event UnityAction CloseButtonAction
    {
        add => closeButton.onClick.AddListener(value);
        remove => closeButton.onClick.RemoveListener(value);
    }

    public void SetDefaultNpcInfo(string npcName, string npcRole)
    {
        npcNameText.text = npcName;
        npcRoleText.text = npcRole;
    }

    public void ShowNextButton(bool index)
    {
        nextButton.gameObject.SetActive(index);
    }

    public void ShowOkButton(bool index)
    {
        okButton.gameObject.SetActive(index);
    }
}
