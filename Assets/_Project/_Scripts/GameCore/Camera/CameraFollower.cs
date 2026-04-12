using UnityEngine;

namespace GameCore.CameraSystem
{
    public class CameraFollower : MonoBehaviour
    {
        [SerializeField] private RectTransform _target;
        [SerializeField] private RectTransform _container;
        [SerializeField] private float _smoothSpeed = 5f;

        private float _initialTargetY;
        private bool _initialized;

        private void LateUpdate()
        {
            if (_target == null || _container == null) return;

            if (!_initialized)
            {
                _initialTargetY = _target.anchoredPosition.y;
                _initialized = true;
            }

            float delta = _target.anchoredPosition.y - _initialTargetY;
            float targetY = -delta;

            Vector2 pos = _container.anchoredPosition;
            pos.y = Mathf.Lerp(pos.y, targetY, _smoothSpeed * Time.deltaTime);
            _container.anchoredPosition = pos;
        }
    }
}