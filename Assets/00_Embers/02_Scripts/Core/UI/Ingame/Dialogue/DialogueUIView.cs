using System;
using Michsky.UI.Reach;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueUIView : MonoBehaviour
{
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private TMP_Text npcRoleText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private ButtonManager nextButton;
    [SerializeField] private ButtonManager okButton;
    [SerializeField] private ButtonManager closeButton;

    public string DialogueText
    {
        get => dialogueText.text;
        set
        {
            dialogueText.text = value;
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
