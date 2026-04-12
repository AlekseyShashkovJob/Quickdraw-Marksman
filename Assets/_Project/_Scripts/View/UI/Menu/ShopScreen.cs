using GameCore.Constants;
using GameCore.Data;
using TMPro;
using UnityEngine;
using View.Button;

namespace View.UI.Menu
{
    public class ShopScreen : UIScreen
    {
        [SerializeField] private CustomButton _back;
        [SerializeField] private TMP_Text _coinsText;
        [SerializeField] private WeaponItem[] _items;

        private void OnEnable()
        {
            _back.AddListener(BackToMenu);
            RefreshCoins();

            foreach (var item in _items)
                item.Init(OnItemAction);
        }

        private void OnDisable()
        {
            _back.RemoveListener(BackToMenu);
        }

        private void OnItemAction(WeaponItem item)
        {
            if (!WeaponInventory.IsOwned(item.Data.id))
                TryBuy(item);
            else
                Select(item);
        }

        private void TryBuy(WeaponItem item)
        {
            int coins = PlayerPrefs.GetInt(GameConstants.TOTAL_COINS_KEY, 0);
            if (coins < item.Data.price) return;

            coins -= item.Data.price;
            PlayerPrefs.SetInt(GameConstants.TOTAL_COINS_KEY, coins);
            PlayerPrefs.Save();

            WeaponInventory.Purchase(item.Data.id);
            WeaponInventory.SetSelected(item.Data.id);

            RefreshAll();
        }

        private void Select(WeaponItem item)
        {
            WeaponInventory.SetSelected(item.Data.id);
            RefreshAll();
        }

        private void RefreshAll()
        {
            RefreshCoins();
            foreach (var it in _items)
                it.Refresh();
        }

        private void RefreshCoins()
        {
            int coins = PlayerPrefs.GetInt(GameConstants.TOTAL_COINS_KEY, 0);
            _coinsText.text = $"{coins}<sprite=0>";
        }

        private void BackToMenu() => CloseScreen();
    }
}