using System.Collections;
using TMPro;
using UnityEngine;

namespace View.UI.Game
{
    public class FloatingScore : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        [Header("Animation")]
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private float _startScale = 0.8f;
        [SerializeField] private float _endScale = 2f;
        [SerializeField] private float _floatUpSpeed = 120f;

        [Header("Colors by Score")]
        [SerializeField] private Color _color1 = Color.white;
        [SerializeField] private Color _color2 = Color.yellow;
        [SerializeField] private Color _color3 = Color.red;

        private RectTransform _rectTransform;

        public void Play(int score, Vector2 anchoredPos)
        {
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.anchoredPosition = anchoredPos;
            _rectTransform.localScale = Vector3.one * _startScale;

            _text.text = $"+{score}";
            _text.color = GetColor(score);

            StartCoroutine(AnimateRoutine());
        }

        private IEnumerator AnimateRoutine()
        {
            float elapsed = 0f;
            Vector2 startPos = _rectTransform.anchoredPosition;
            Color startColor = _text.color;

            while (elapsed < _duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _duration);

                // Масштаб растёт
                float scale = Mathf.Lerp(_startScale, _endScale, t);
                _rectTransform.localScale = Vector3.one * scale;

                // Прозрачность падает
                Color c = startColor;
                c.a = 1f - t;
                _text.color = c;

                // Плывёт вверх
                _rectTransform.anchoredPosition = startPos + Vector2.up * (_floatUpSpeed * t);

                yield return null;
            }

            Destroy(gameObject);
        }

        private Color GetColor(int score)
        {
            return score switch
            {
                1 => _color1,
                2 => _color2,
                >= 3 => _color3,
                _ => _color1
            };
        }
    }
}