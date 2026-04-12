using System;
using GameCore.Targets;
using UnityEngine;

namespace GameCore.Shooting
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _speed = 2500f;
        [SerializeField] private float _maxDistance = 3000f;

        private RectTransform _rectTransform;
        private Vector2 _direction;
        private float _distanceTraveled;
        private bool _isFlying;

        private Action<Target, int> _onHit;
        private Action _onMiss;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            var rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            var col = GetComponent<BoxCollider2D>();
            col.isTrigger = true;
        }

        public void Fire(Vector2 startPos, Vector2 direction, Action<Target, int> onHit, Action onMiss)
        {
            _rectTransform.anchoredPosition = startPos;
            _direction = direction.normalized;
            _distanceTraveled = 0f;
            _isFlying = true;
            _onHit = onHit;
            _onMiss = onMiss;

            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            _rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);

            transform.SetAsLastSibling();
        }

        public void Stop()
        {
            _isFlying = false;
            _onHit = null;
            _onMiss = null;
        }

        private void Update()
        {
            if (!_isFlying) return;

            Vector2 delta = _direction * _speed * Time.deltaTime;
            _rectTransform.anchoredPosition += delta;
            _distanceTraveled += delta.magnitude;

            if (_distanceTraveled >= _maxDistance)
            {
                _isFlying = false;
                _onMiss?.Invoke();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isFlying) return;

            if (other.CompareTag("Target"))
            {
                _isFlying = false;
                Target target = other.GetComponent<Target>();
                if (target != null)
                {
                    int score = target.GetHitScore(_rectTransform.anchoredPosition.y);
                    _onHit?.Invoke(target, score);
                }
            }
        }
    }
}

