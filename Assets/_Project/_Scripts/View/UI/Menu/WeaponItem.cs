using System;
using GameCore.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.Menu
{
    public class WeaponItem : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private WeaponData _data;

        [Header("UI")]
        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _priceObject;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private Button.CustomButton _button;
        [SerializeField] private TMP_Text _buttonText;

        private Action<WeaponItem> _onAction;

        public WeaponData Data => _data;

        private void OnEnable() => _button.AddListener(OnClick);
        private void OnDisable() => _button.RemoveListener(OnClick);

        public void Init(Action<WeaponItem> onAction)
        {
            _onAction = onAction;

            _icon.sprite = _data.icon;
            _priceText.text = $"{_data.price}<sprite=0>";

            if (_data.price <= 0 && !WeaponInventory.IsOwned(_data.id))
            {
                WeaponInventory.Purchase(_data.id);

                if (string.IsNullOrEmpty(WeaponInventory.GetSelectedId()))
                    WeaponInventory.SetSelected(_data.id);
            }

            Refresh();
        }

        public void Refresh()
        {
            bool owned = WeaponInventory.IsOwned(_data.id);
            bool selected = WeaponInventory.GetSelectedId() == _data.id;

            _priceObject.SetActive(!owned);

            if (!owned)
                _buttonText.text = "BUY";
            else if (selected)
                _buttonText.text = "SELECTED";
            else
                _buttonText.text = "SELECT";
        }

        private void OnClick() => _onAction?.Invoke(this);
    }
}