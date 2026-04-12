using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Player
{
    [RequireComponent(typeof(Image))]
    public class AimLine : MonoBehaviour
    {
        [SerializeField] private float _alpha = 0.35f;

        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();

            Color c = _image.color;
            c.a = _alpha;
            _image.color = c;
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}

