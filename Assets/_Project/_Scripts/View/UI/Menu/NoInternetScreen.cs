using UnityEngine;
using System;

namespace View.UI.Menu
{
    public class NoInternetScreen : UIScreen
    {
        [SerializeField] private Misc.SceneManagment.SceneLoader _sceneLoader;
        [SerializeField] private Button.CustomButton _backPortrait;
        [SerializeField] private Button.CustomButton _backLandscape;

        [SerializeField] private GameObject _backgroundPortrait;
        [SerializeField] private GameObject _backgroundLandscape;

        private Vector2Int _lastResolution;

        private void Update()
        {
            Vector2Int currentResolution = new Vector2Int(Screen.width, Screen.height);
            if (currentResolution != _lastResolution)
            {
                _lastResolution = currentResolution;
                UpdateBackground();
            }
        }

        public override void StartScreen()
        {
            base.StartScreen();

            _lastResolution = new Vector2Int(Screen.width, Screen.height);
            UpdateBackground();
        }

        private void OnEnable()
        {
            _backPortrait.AddListener(Restart);
            _backLandscape.AddListener(Restart);
        }

        private void OnDisable()
        {
            _backPortrait.RemoveListener(Restart);
            _backLandscape.RemoveListener(Restart);
        }

        private void Restart()
        {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            _sceneLoader.ChangeScene(currentScene);
            CloseScreen();
        }

        private void UpdateBackground()
        {
            bool isPortrait = Screen.height >= Screen.width;

            _backgroundPortrait.SetActive(isPortrait);
            _backgroundLandscape.SetActive(!isPortrait);
        }
    }
}