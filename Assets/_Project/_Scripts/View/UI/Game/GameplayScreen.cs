using UnityEngine;
using View.Button;

namespace View.UI.Game
{
    public class GameplayScreen : UIScreen
    {
        [SerializeField] private CustomButton _pause;
        [SerializeField] private UIScreen _pauseScreen;

        private void OnEnable() => _pause.AddListener(Pause);
        private void OnDisable() => _pause.RemoveListener(Pause);

        private void Pause() => _pauseScreen.StartScreen();
    }
}