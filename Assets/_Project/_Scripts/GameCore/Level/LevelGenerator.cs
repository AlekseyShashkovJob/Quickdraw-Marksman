using System.Collections.Generic;
using GameCore.Pool;
using UnityEngine;

namespace GameCore.Level
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private ObjectPool _platformPool;
        [SerializeField] private RectTransform _contentArea;

        [Header("Level Settings")]
        [SerializeField] private int _minPlatforms = 15;
        [SerializeField] private int _maxPlatforms = 25;
        [SerializeField] private int _visiblePlatforms = 5;

        [Header("Spacing")]
        [SerializeField] private float _verticalSpacing = 350f;

        private readonly List<Vector2> _positions = new();
        private readonly Dictionary<int, GameObject> _activePlatforms = new();

        private int _windowStart;
        private int _windowEnd;

        public IReadOnlyList<Vector2> Positions => _positions;
        public int TotalPlatforms => _positions.Count;

        public bool IsLeftSide(int index) => index % 2 == 0;

        public void Generate()
        {
            Clear();
            CreatePositions();

            _windowStart = 0;
            _windowEnd = Mathf.Min(_visiblePlatforms - 1, _positions.Count - 1);

            for (int i = _windowStart; i <= _windowEnd; i++)
                Activate(i);
        }

        public void AdvanceTo(int currentIndex)
        {
            int newStart = Mathf.Max(0, currentIndex - 1);
            int newEnd = Mathf.Min(
                currentIndex + _visiblePlatforms - 2,
                _positions.Count - 1
            );

            for (int i = _windowStart; i < newStart; i++)
                Deactivate(i);

            for (int i = _windowEnd + 1; i <= newEnd; i++)
                Activate(i);

            _windowStart = newStart;
            _windowEnd = newEnd;
        }

        public GameObject GetActivePlatform(int index)
        {
            _activePlatforms.TryGetValue(index, out GameObject obj);
            return obj;
        }

        private void CreatePositions()
        {
            Rect bounds = _contentArea.rect;
            Vector2 platSize = _platformPool.PrefabSize;
            float halfW = platSize.x * 0.5f;
            float halfH = platSize.y * 0.5f;

            float leftX = bounds.xMin + halfW;
            float rightX = bounds.xMax - halfW;

            float startY = bounds.yMin + halfH;

            int count = Random.Range(_minPlatforms, _maxPlatforms + 1);
            if (count % 2 == 0) count++;
            if (count < 3) count = 3;

            for (int i = 0; i < count; i++)
            {
                bool left = (i % 2 == 0);
                float x = left ? leftX : rightX;
                float y = startY + _verticalSpacing * i;
                _positions.Add(new Vector2(x, y));
            }
        }

        private void Activate(int index)
        {
            if (_activePlatforms.ContainsKey(index)) return;
            if (index < 0 || index >= _positions.Count) return;

            GameObject obj = _platformPool.Get();
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = _positions[index];

            Platform platform = obj.GetComponent<Platform>();
            platform.Setup(index);

            _activePlatforms[index] = obj;
        }

        private void Deactivate(int index)
        {
            if (_activePlatforms.TryGetValue(index, out GameObject obj))
            {
                _platformPool.Return(obj);
                _activePlatforms.Remove(index);
            }
        }

        private void Clear()
        {
            _platformPool.ReturnAll();
            _positions.Clear();
            _activePlatforms.Clear();
            _windowStart = 0;
            _windowEnd = 0;
        }
    }
}