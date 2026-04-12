using Misc.Data;
using UnityEngine;
using View.Button;

namespace View.UI.Menu
{
    public class ChooseMapScreen : UIScreen
    {
        [SerializeField] private Misc.SceneManagment.SceneLoader _sceneLoader;

        [Space, Header("Buttons")]
        [SerializeField] private CustomButton _day;
        [SerializeField] private CustomButton _night;
        [SerializeField] private CustomButton _back;

        private void OnEnable()
        {
            _day.AddListener(OpenDay);
            _night.AddListener(OpenNight);
            _back.AddListener(BackToMenu);
        }

        private void OnDisable()
        {
            _day.RemoveListener(OpenDay);
            _night.RemoveListener(OpenNight);
            _back.RemoveListener(BackToMenu);
        }

        private void OpenDay() => LoadMap(MapType.Day);
        private void OpenNight() => LoadMap(MapType.Night);

        private void LoadMap(MapType mapType)
        {
            MapData.SelectedMap = mapType;
            _sceneLoader.ChangeScene(SceneConstants.GAME_SCENE);
            CloseScreen();
        }

        private void BackToMenu() => CloseScreen();
    }
}