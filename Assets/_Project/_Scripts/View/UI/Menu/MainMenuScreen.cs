using UnityEngine;
using View.Button;

namespace View.UI.Menu
{
    public class MainMenuScreen : UIScreen
    {
        [Space, Header("Screens")]
        [SerializeField] private UIScreen _chooseMapScreen;
        [SerializeField] private UIScreen _optionsScreen;
        [SerializeField] private UIScreen _privacyScreen;
        [SerializeField] private UIScreen _shopScreen;

        [Space, Header("Buttons")]
        [SerializeField] private CustomButton _startGame;
        [SerializeField] private CustomButton _settings;
        [SerializeField] private CustomButton _privacy;
        [SerializeField] private CustomButton _shop;

        private void OnEnable()
        {
            _startGame.AddListener(OpenChooseMap);
            _settings.AddListener(OpenOptions);
            _privacy.AddListener(OpenPrivacy);
            _shop.AddListener(OpenShop);
        }

        private void OnDisable()
        {
            _startGame.RemoveListener(OpenChooseMap);
            _settings.RemoveListener(OpenOptions);
            _privacy.RemoveListener(OpenPrivacy);
            _shop.RemoveListener(OpenShop);
        }

        public override void StartScreen()
        {
            base.StartScreen();
        }

        private void OpenChooseMap() => _chooseMapScreen.StartScreen();
        private void OpenOptions() => _optionsScreen.StartScreen();
        private void OpenPrivacy() => _privacyScreen.StartScreen();
        private void OpenShop() => _shopScreen.StartScreen();
    }
}