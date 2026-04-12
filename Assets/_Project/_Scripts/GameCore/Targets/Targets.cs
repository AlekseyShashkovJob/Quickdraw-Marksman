using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Targets
{
    public enum HitZone { Legs, Body, Head }

    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Target : MonoBehaviour
    {
        [Header("Position on Platform")]
        [SerializeField] private float _standOffsetY = 60f;
        [SerializeField] private float _edgeOffsetX = 40f;

        [Header("Visuals")]
        [SerializeField] private Image _image;
        [SerializeField] private Sprite[] _sprites;

        [Header("Random Size")]
        [SerializeField] private float _sizeMin = 80f;
        [SerializeField] private float _sizeMax = 130f;

        [Header("Hit Zones")]
        [SerializeField] private float _legsThreshold = 0.33f;
        [SerializeField] private float _bodyThreshold = 0.66f;

        [Header("Score per Zone")]
        [SerializeField] private int _legsScore = 1;
        [SerializeField] private int _bodyScore = 2;
        [SerializeField] private int _headScore = 3;

        private RectTransform _rectTransform;
        private BoxCollider2D _collider;

        public int PlatformIndex { get; private set; }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _collider = GetComponent<BoxCollider2D>();
            _collider.isTrigger = true;
        }

        public void Setup(Vector2 platformPos, int platformIndex, bool isLeftSide)
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();

            PlatformIndex = platformIndex;

            float xOffset = isLeftSide ? _edgeOffsetX : -_edgeOffsetX;
            _rectTransform.anchoredPosition = new Vector2(
                platformPos.x + xOffset,
                platformPos.y + _standOffsetY
            );

            Vector3 s = transform.localScale;
            s.x = isLeftSide ? 1f : -1f;
            transform.localScale = s;

            if (_sprites != null && _sprites.Length > 0)
                _image.sprite = _sprites[Random.Range(0, _sprites.Length)];

            float size = Random.Range(_sizeMin, _sizeMax);
            _rectTransform.sizeDelta = new Vector2(size, size);
            _collider.size = new Vector2(size, size);
        }

        public int GetHitScore(float bulletY)
        {
            float targetY = _rectTransform.anchoredPosition.y;
            float halfH = _rectTransform.sizeDelta.y * 0.5f;
            float bottom = targetY - halfH;
            float height = _rectTransform.sizeDelta.y;

            float relative = Mathf.Clamp01((bulletY - bottom) / height);

            HitZone zone;
            if (relative < _legsThreshold)
                zone = HitZone.Legs;
            else if (relative < _bodyThreshold)
                zone = HitZone.Body;
            else
                zone = HitZone.Head;

            Debug.Log($"[Target] Hit zone: {zone} (relative={relative:F2}) -> +{GetScore(zone)}");

            return GetScore(zone);
        }

        private int GetScore(HitZone zone)
        {
            return zone switch
            {
                HitZone.Legs => _legsScore,
                HitZone.Body => _bodyScore,
                HitZone.Head => _headScore,
                _ => _bodyScore
            };
        }
    }
}