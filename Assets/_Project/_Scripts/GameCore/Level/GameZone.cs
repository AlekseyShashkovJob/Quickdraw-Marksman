using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameCore.Level
{
    [RequireComponent(typeof(Image))]
    public class GameZone : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private AudioClip _shootSound;

        private RectTransform _rectTransform;
        public RectTransform RectTransform =>
            _rectTransform ??= GetComponent<RectTransform>();

        public event Action OnTap;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsGameActive)
                return;

            if (_shootSound != null)
                Misc.Services.SoundManager.Instance?.PlayClick(_shootSound);

            OnTap?.Invoke();
        }
    }
}