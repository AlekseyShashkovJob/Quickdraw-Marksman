using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Pool
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private RectTransform _container;
        [SerializeField] private int _initialSize = 4;

        private readonly Queue<GameObject> _available = new Queue<GameObject>();
        private readonly List<GameObject> _active = new List<GameObject>();

        private void Awake()
        {
            Prewarm();
        }

        public GameObject Get()
        {
            GameObject obj = _available.Count > 0
                ? _available.Dequeue()
                : CreateObject();

            obj.SetActive(true);
            _active.Add(obj);
            return obj;
        }

        public Vector2 PrefabSize
        {
            get
            {
                RectTransform rect = _prefab.GetComponent<RectTransform>();
                return rect.rect.size;
            }
        }

        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            _active.Remove(obj);
            _available.Enqueue(obj);
        }

        public void ReturnAll()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                _active[i].SetActive(false);
                _available.Enqueue(_active[i]);
            }
            _active.Clear();
        }

        private void Prewarm()
        {
            for (int i = 0; i < _initialSize; i++)
                _available.Enqueue(CreateObject());
        }

        private GameObject CreateObject()
        {
            GameObject obj = Instantiate(_prefab, _container);
            obj.SetActive(false);
            return obj;
        }
    }
}

