using UnityEngine;
using TMPro;
using View.Button;

namespace View.UI.Game
{
    public class LoseScreen : UIScreen
    {
        [SerializeField] private CustomButton _back;
        [SerializeField] private CustomButton _restart;

        [SerializeField] private Misc.SceneManagment.SceneLoader _sceneLoader;

        [SerializeField] private TMP_Text _currentScoreText;
        [SerializeField] private TMP_Text _bestScoreText;

        private System.Action _onRestart;

        private void OnEnable()
        {
            _back.AddListener(BackToMenu);
            _restart.AddListener(Restart);
        }

        private void OnDisable()
        {
            _back.RemoveListener(BackToMenu);
            _restart.RemoveListener(Restart);
        }

        public void Show(int currentScore, int bestScore, System.Action onRestart)
        {
            _onRestart = onRestart;

            base.StartScreen();
            Time.timeScale = 0.0f;

            _currentScoreText.text = $"SCORE <color=#FFDA0DFF>{currentScore}</color>";
            _bestScoreText.text = $"BEST <color=#FFDA0DFF>{bestScore}</color>";
        }

        private void BackToMenu()
        {
            Time.timeScale = 1.0f;
            _sceneLoader.ChangeScene(Misc.Data.SceneConstants.MENU_SCENE);
            CloseScreen();
        }

        private void Restart()
        {
            //Time.timeScale = 1.0f;
            _onRestart?.Invoke();
            CloseScreen();
        }
    }
}