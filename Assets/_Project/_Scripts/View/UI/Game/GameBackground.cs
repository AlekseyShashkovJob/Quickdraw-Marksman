using Misc.Data;
using UnityEngine;

namespace View.UI.Game
{
    public class GameBackground : MonoBehaviour
    {
        [SerializeField] private GameObject _bgDay;
        [SerializeField] private GameObject _bgNight;

        private void Start()
        {
            _bgDay.SetActive(false);
            _bgNight.SetActive(false);

            GetBackground(MapData.SelectedMap).SetActive(true);
        }

        private GameObject GetBackground(MapType mapType)
        {
            return mapType switch
            {
                MapType.Day => _bgDay,
                MapType.Night => _bgNight,
                _ => _bgDay
            };
        }
    }
}