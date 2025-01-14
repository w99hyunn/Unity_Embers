using System;
using Michsky.UI.Reach;
using TMPro;
using UnityEngine;

namespace STARTING
{
    /// <summary>
    /// 아이템을 버리거나 수량 나눌 때 띄우는 팝업
    /// </summary>
    public class InventoryPopupUI : MonoBehaviour
    {
        // 1. 아이템 버리기 확인 팝업
        [Header("Confirmation Popup")]
        [SerializeField] private ModalWindowManager _confirmationPopupObject;
        [SerializeField] private TMP_Text   _confirmationItemNameText;
        [SerializeField] private ButtonManager _confirmationOkButton;     // Ok
        [SerializeField] private ButtonManager _confirmationCancelButton; // Cancel

        // 2. 수량 입력 팝업
        [Header("Amount Input Popup")]
        [SerializeField] private ModalWindowManager _amountInputPopupObject;
        [SerializeField] private TMP_Text       _amountInputItemNameText;
        [SerializeField] private TMP_InputField _amountInputField;
        [SerializeField] private ButtonManager _amountPlusButton;        // +
        [SerializeField] private ButtonManager _amountMinusButton;       // -
        [SerializeField] private ButtonManager _amountInputOkButton;     // Ok
        [SerializeField] private ButtonManager _amountInputCancelButton; // Cancel

        // 확인 버튼 눌렀을 때 동작할 이벤트
        private event Action OnConfirmationOK;
        private event Action<int> OnAmountInputOK;

        // 수량 입력 제한 개수
        private int _maxAmount;
        
        private void Awake()
        {
            InitUIEvents();
            HideConfirmationPopup();
            HideAmountInputPopup();
        }

        /// <summary>
        /// 버릴건지 팝업
        /// </summary>
        public void OpenConfirmationPopup(Action okCallback, string itemName)
        {
            ShowConfirmationPopup(itemName);
            SetConfirmationOKEvent(okCallback);
        }
        
        /// <summary> 수량 입력 팝업 띄우기 </summary>
        public void OpenAmountInputPopup(Action<int> okCallback, int currentAmount, string itemName)
        {
            _maxAmount = currentAmount - 1;
            _amountInputField.text = "1";
            
            ShowAmountInputPopup(itemName);
            SetAmountInputOKEvent(okCallback);
        }

        private void InitUIEvents()
        {
            // 1. 버릴건지 팝업
            _confirmationOkButton.onClick.AddListener(HideConfirmationPopup);
            _confirmationOkButton.onClick.AddListener(() => OnConfirmationOK?.Invoke());
            
            _confirmationCancelButton.onClick.AddListener(HideConfirmationPopup);

            // 2. 수량 입력 팝업
            _amountInputOkButton.onClick.AddListener(HideAmountInputPopup);
            _amountInputOkButton.onClick.AddListener(() => OnAmountInputOK?.Invoke(int.Parse(_amountInputField.text)));
            
            _amountInputCancelButton.onClick.AddListener(HideAmountInputPopup);

            // - 버튼
            _amountMinusButton.onClick.AddListener(() =>
            {
                int.TryParse(_amountInputField.text, out int amount);
                if (amount > 1)
                {
                    // Shift 누르면 10씩 감소
                    int nextAmount = Input.GetKey(KeyCode.LeftShift) ? amount - 10 : amount - 1;
                    if(nextAmount < 1)
                        nextAmount = 1;
                    _amountInputField.text = nextAmount.ToString();
                }
            });

            // + 버튼
            _amountPlusButton.onClick.AddListener(() =>
            {
                int.TryParse(_amountInputField.text, out int amount);
                if (amount < _maxAmount)
                {
                    // Shift 누르면 10씩 증가
                    int nextAmount = Input.GetKey(KeyCode.LeftShift) ? amount + 10 : amount + 1;
                    if (nextAmount > _maxAmount)
                        nextAmount = _maxAmount;
                    _amountInputField.text = nextAmount.ToString();
                }
            });

            // 입력 값 범위 제한
            _amountInputField.onValueChanged.AddListener(str =>
            {
                int.TryParse(str, out int amount);
                bool flag = false;

                if (amount < 1)
                {
                    flag = true;
                    amount = 1;
                }
                else if (amount > _maxAmount)
                {
                    flag = true;
                    amount = _maxAmount;
                }

                if(flag)
                    _amountInputField.text = amount.ToString();
            });
        }

        private void ShowConfirmationPopup(string itemName)
        {
            _confirmationItemNameText.text = itemName;
            _confirmationPopupObject.OpenWindow();
        }
        private void HideConfirmationPopup() => _confirmationPopupObject.CloseWindow();

        private void ShowAmountInputPopup(string itemName)
        {
            _amountInputItemNameText.text = itemName;
            _amountInputPopupObject.OpenWindow();
        }

        private void HideAmountInputPopup() => _amountInputPopupObject.CloseWindow();

        private void SetConfirmationOKEvent(Action handler) => OnConfirmationOK = handler;
        private void SetAmountInputOKEvent(Action<int> handler) => OnAmountInputOK = handler;
    }
}