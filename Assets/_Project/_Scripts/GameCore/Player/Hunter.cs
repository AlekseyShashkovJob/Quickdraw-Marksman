using System;
using System.Collections;
using UnityEngine;

namespace GameCore.Player
{
    [RequireComponent(typeof(RectTransform))]
    public class Hunter : MonoBehaviour
    {
        [Header("Weapon Slot")]
        [SerializeField] private RectTransform _weaponSlot;

        [Header("Position on Platform")]
        [SerializeField] private float _standOffsetY = 60f;
        [SerializeField] private float _edgeOffsetX = 80f;

        [Header("Jump")]
        [SerializeField] private float _moveSpeed = 800f;
        [SerializeField] private float _jumpHeight = 150f;

        private RectTransform _rectTransform;
        private Weapon _weapon;
        private int _currentPlatformIndex;
        private bool _isJumping;

        public RectTransform RectTransform =>
            _rectTransform ??= GetComponent<RectTransform>();

        public int CurrentPlatformIndex => _currentPlatformIndex;
        public Weapon Weapon => _weapon;
        public bool IsJumping => _isJumping;

        public void EquipWeapon(GameObject weaponPrefab)
        {
            if (_weapon != null)
                Destroy(_weapon.gameObject);

            GameObject obj = Instantiate(weaponPrefab, _weaponSlot);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;

            _weapon = obj.GetComponent<Weapon>();
        }

        public void Initialize(Vector2 platformPos, int platformIndex, bool isLeftSide)
        {
            _rectTransform = GetComponent<RectTransform>();
            PlaceOnPlatform(platformPos, platformIndex, isLeftSide);
            _weapon.Show();
            _weapon.StartSwing();
        }

        public void PlaceOnPlatform(Vector2 platformPos, int platformIndex, bool isLeftSide)
        {
            float xOffset = isLeftSide ? _edgeOffsetX : -_edgeOffsetX;

            _rectTransform.anchoredPosition = new Vector2(
                platformPos.x + xOffset,
                platformPos.y + _standOffsetY
            );

            _currentPlatformIndex = platformIndex;
            SetFacing(isLeftSide);
        }

        public void JumpToPlatform(Vector2 platformPos, int platformIndex, bool isLeftSide, Action onLanded)
        {
            if (_isJumping) return;

            _weapon.Hide();

            float xOffset = isLeftSide ? _edgeOffsetX : -_edgeOffsetX;
            Vector2 animTarget = new Vector2(
                platformPos.x + xOffset,
                platformPos.y + _standOffsetY
            );

            StartCoroutine(JumpRoutine(animTarget, platformPos, platformIndex, isLeftSide, onLanded));
        }

        private IEnumerator JumpRoutine(
            Vector2 animTarget, Vector2 platformPos,
            int platformIndex, bool isLeftSide, Action onLanded)
        {
            _isJumping = true;

            Vector2 startPos = _rectTransform.anchoredPosition;
            float distance = Vector2.Distance(startPos, animTarget);
            float duration = distance / _moveSpeed;
            if (duration < 0.05f) duration = 0.05f;

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float smoothT = Mathf.SmoothStep(0f, 1f, t);

                Vector2 pos = Vector2.Lerp(startPos, animTarget, smoothT);
                pos.y += _jumpHeight * 4f * t * (1f - t);

                _rectTransform.anchoredPosition = pos;
                yield return null;
            }

            PlaceOnPlatform(platformPos, platformIndex, isLeftSide);

            _isJumping = false;
            onLanded?.Invoke();
        }

        private void SetFacing(bool isLeftSide)
        {
            Vector3 s = transform.localScale;
            s.x = isLeftSide ? 1f : -1f;
            transform.localScale = s;
        }
    }
}