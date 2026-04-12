using UnityEngine;

namespace GameCore.Player
{
    public class Weapon : MonoBehaviour
    {
        [Header("Swing")]
        [SerializeField] private float _angleMin = 10f;
        [SerializeField] private float _angleMax = 80f;
        [SerializeField] private float _swingSpeed = 90f;

        [Header("References")]
        [SerializeField] private RectTransform _firePoint;
        [SerializeField] private AimLine _aimLine;

        private float _currentAngle;
        private int _direction = 1;
        private bool _isSwinging;

        public Vector2 FirePointWorld => _firePoint.position;
        public Vector2 FireDirection => transform.right;
        public float CurrentAngle => _currentAngle;
        public bool IsSwinging => _isSwinging;
        public RectTransform FirePoint => _firePoint;

        public void SetSwingSpeed(float speed)
        {
            _swingSpeed = speed;
        }

        public void Hide()
        {
            StopSwing();
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void StartSwing()
        {
            _currentAngle = _angleMin;
            _direction = 1;
            _isSwinging = true;
            _aimLine.Show();
            ApplyRotation();
        }

        public void StopSwing()
        {
            _isSwinging = false;
            _aimLine.Hide();
        }

        public void LockAngle()
        {
            _isSwinging = false;
        }

        private void Update()
        {
            if (!_isSwinging) return;

            _currentAngle += _direction * _swingSpeed * Time.deltaTime;

            if (_currentAngle >= _angleMax)
            {
                _currentAngle = _angleMax;
                _direction = -1;
            }
            else if (_currentAngle <= _angleMin)
            {
                _currentAngle = _angleMin;
                _direction = 1;
            }

            ApplyRotation();
        }

        private void ApplyRotation()
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, _currentAngle);
        }
    }
}