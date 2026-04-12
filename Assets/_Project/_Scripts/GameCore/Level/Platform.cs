using Misc.Data;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Level
{
    [RequireComponent(typeof(RectTransform))]
    public class Platform : MonoBehaviour
    {
        [Header("Theme Sprites")]
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _daySprite;
        [SerializeField] private Sprite _nightSprite;

        public int Index { get; private set; }

        private RectTransform _rectTransform;
        public RectTransform RectTransform =>
            _rectTransform ??= GetComponent<RectTransform>();

        public void Setup(int index)
        {
            Index = index;
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            _image.sprite = MapData.SelectedMap switch
            {
                MapType.Day => _daySprite,
                MapType.Night => _nightSprite,
                _ => _daySprite
            };
        }
    }
}