using GameCore.Constants;
using GameCore.Data;
using GameCore.Level;
using GameCore.Player;
using GameCore.Pool;
using GameCore.Shooting;
using GameCore.Targets;
using Misc.Data;
using Misc.SceneManagment;
using TMPro;
using UnityEngine;
using View.UI.Game;

namespace GameCore
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public int CurrentScore { get; private set; }
        public int BestScore { get; private set; }
        public int TotalCoins { get; private set; }
        public bool IsGameActive { get; private set; }

        [Header("UI")]
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _coinsText;
        [SerializeField] private LevelProgressBar _progressBar;

        [Header("Screens")]
        [SerializeField] private VictoryScreen _winScreen;
        [SerializeField] private LoseScreen _loseScreen;

        [Header("References")]
        [SerializeField] private SceneLoader _sceneLoader;
        [SerializeField] private LevelGenerator _levelGenerator;
        [SerializeField] private Hunter _hunter;
        [SerializeField] private GameZone _gameZone;

        [Header("Pools")]
        [SerializeField] private ObjectPool _bulletPool;
        [SerializeField] private ObjectPool _targetPool;

        [Header("Difficulty")]
        [SerializeField] private float _baseSwingSpeed = 80f;
        [SerializeField] private float _maxSwingSpeed = 200f;

        [Header("Effects")]
        [SerializeField] private FloatingScore _floatingScorePrefab;

        [Header("Weapons")]
        [SerializeField] private WeaponData[] _allWeapons;

        private const int START_PLATFORM = 1;

        private Target _currentTarget;
        private Bullet _activeBullet;
        private bool _isShooting;
        private int _totalTargets;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                LoadData();
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;

            if (_gameZone != null)
                _gameZone.OnTap -= HandleShoot;
        }

        private void Start()
        {
            Time.timeScale = 1f;

            _hunter.gameObject.SetActive(false);
            _gameZone.OnTap += HandleShoot;

            EquipSelectedWeapon();

            _levelGenerator.Generate();
            StartGame();
        }

        public void StartGame()
        {
            CurrentScore = 0;
            _isShooting = false;
            IsGameActive = true;

            _totalTargets = _levelGenerator.TotalPlatforms - START_PLATFORM - 1;

            _hunter.gameObject.SetActive(true);
            PlaceHunterOnStart();
            SpawnTarget(_hunter.CurrentPlatformIndex + 1);
            ApplyDifficulty(_hunter.CurrentPlatformIndex);

            UpdateUI();
            UpdateProgress();
        }

        public void AddScore(int points)
        {
            if (!IsGameActive) return;
            CurrentScore += points;
            UpdateUI();
        }

        public void AddCoin()
        {
            TotalCoins++;
            SaveData();
            UpdateUI();
        }

        public void WinGame()
        {
            if (!IsGameActive) return;
            IsGameActive = false;

            _hunter.Weapon.StopSwing();
            _hunter.gameObject.SetActive(false);
            SaveSession();

            _winScreen.Show(CurrentScore, BestScore, RestartGame);
        }

        public void LoseGame()
        {
            if (!IsGameActive) return;
            IsGameActive = false;

            _hunter.Weapon.StopSwing();
            _hunter.gameObject.SetActive(false);
            SaveSession();

            _loseScreen.Show(CurrentScore, BestScore, RestartGame);
        }
        private void EquipSelectedWeapon()
        {
            string selectedId = WeaponInventory.GetSelectedId();

            foreach (var data in _allWeapons)
            {
                if (data.id == selectedId)
                {
                    _hunter.EquipWeapon(data.weaponPrefab);
                    return;
                }
            }

            if (_allWeapons.Length > 0)
                _hunter.EquipWeapon(_allWeapons[0].weaponPrefab);
        }

        private void HandleShoot()
        {
            if (!IsGameActive || _isShooting) return;

            _isShooting = true;
            _hunter.Weapon.LockAngle();

            float firePointOffset = _hunter.Weapon.FirePoint.localEulerAngles.z;
            float totalAngle = _hunter.Weapon.CurrentAngle + firePointOffset;
            float angleRad = totalAngle * Mathf.Deg2Rad;

            bool facingRight = _levelGenerator.IsLeftSide(_hunter.CurrentPlatformIndex);

            Vector2 dir = new Vector2(
                Mathf.Cos(angleRad) * (facingRight ? 1f : -1f),
                Mathf.Sin(angleRad)
            ).normalized;

            Vector2 startPos = GetFirePointInContainer();

            GameObject bulletObj = _bulletPool.Get();
            _activeBullet = bulletObj.GetComponent<Bullet>();
            _activeBullet.Fire(startPos, dir, OnHit, OnMiss);
        }

        private void OnHit(Target target, int score)
        {
            ReturnBullet();

            Misc.Services.VibroManager.Vibrate();

            int hitPlatform = target.PlatformIndex;

            ShowFloatingScore(score, target.GetComponent<RectTransform>());

            _targetPool.Return(target.gameObject);
            _currentTarget = null;

            AddScore(score);
            AddCoin();

            bool isLeft = _levelGenerator.IsLeftSide(hitPlatform);

            _hunter.JumpToPlatform(
                _levelGenerator.Positions[hitPlatform],
                hitPlatform,
                isLeft,
                () => OnHunterLanded(hitPlatform)
            );
        }

        private void ShowFloatingScore(int score, RectTransform targetRect)
        {
            Vector2 pos = targetRect.anchoredPosition;
            float halfH = targetRect.sizeDelta.y * 0.5f;
            Vector2 spawnPos = pos + Vector2.up * (halfH + 20f);

            Transform container = _hunter.transform.parent;
            FloatingScore fs = Instantiate(_floatingScorePrefab, container);
            fs.Play(score, spawnPos);
        }

        private void OnHunterLanded(int platformIndex)
        {
            _levelGenerator.AdvanceTo(platformIndex);
            _hunter.transform.SetAsLastSibling();

            UpdateProgress();

            if (platformIndex >= _levelGenerator.TotalPlatforms - 1)
            {
                WinGame();
                return;
            }

            ApplyDifficulty(platformIndex);
            SpawnTarget(platformIndex + 1);

            _hunter.Weapon.Show();
            _hunter.Weapon.StartSwing();
            _isShooting = false;
        }

        private void OnMiss()
        {
            ReturnBullet();
            LoseGame();
        }

        private void ApplyDifficulty(int platformIndex)
        {
            float progress = (float)(platformIndex - START_PLATFORM)
                           / Mathf.Max(1, _totalTargets);

            float speed = Mathf.Lerp(_baseSwingSpeed, _maxSwingSpeed, progress);
            _hunter.Weapon.SetSwingSpeed(speed);
        }

        private void UpdateProgress()
        {
            if (_progressBar == null) return;

            int killed = _hunter.CurrentPlatformIndex - START_PLATFORM;
            float progress = (float)killed / Mathf.Max(1, _totalTargets);
            _progressBar.SetProgress(progress);
        }

        private void PlaceHunterOnStart()
        {
            Vector2 pos = _levelGenerator.Positions[START_PLATFORM];
            bool isLeft = _levelGenerator.IsLeftSide(START_PLATFORM);
            _hunter.Initialize(pos, START_PLATFORM, isLeft);
            _hunter.transform.SetAsLastSibling();
        }

        private void SpawnTarget(int platformIndex)
        {
            if (platformIndex >= _levelGenerator.TotalPlatforms) return;

            bool isLeft = _levelGenerator.IsLeftSide(platformIndex);
            Vector2 platPos = _levelGenerator.Positions[platformIndex];

            GameObject obj = _targetPool.Get();
            Target target = obj.GetComponent<Target>();
            target.Setup(platPos, platformIndex, isLeft);

            obj.transform.SetAsLastSibling();
            _currentTarget = target;

            _hunter.transform.SetAsLastSibling();
        }

        private Vector2 GetFirePointInContainer()
        {
            Transform container = _hunter.transform.parent;
            return container.InverseTransformPoint(_hunter.Weapon.FirePoint.position);
        }

        private void ReturnBullet()
        {
            if (_activeBullet != null)
            {
                _activeBullet.Stop();
                _bulletPool.Return(_activeBullet.gameObject);
                _activeBullet = null;
            }
        }

        private void RestartGame()
        {
            Time.timeScale = 1f;
            _sceneLoader.ChangeScene(SceneConstants.GAME_SCENE);
        }

        private void SaveSession()
        {
            TotalCoins += CurrentScore;
            if (CurrentScore > BestScore)
                BestScore = CurrentScore;
            SaveData();
        }

        private void UpdateUI()
        {
            if (_scoreText) _scoreText.text = $"SCORE: {CurrentScore}";
            if (_coinsText) _coinsText.text = $"{TotalCoins}<sprite=0>";
        }

        private void SaveData()
        {
            PlayerPrefs.SetInt(GameConstants.BEST_SCORE_KEY, BestScore);
            PlayerPrefs.SetInt(GameConstants.TOTAL_COINS_KEY, TotalCoins);
            PlayerPrefs.Save();
        }

        private void LoadData()
        {
            BestScore = PlayerPrefs.GetInt(GameConstants.BEST_SCORE_KEY, 0);
            TotalCoins = PlayerPrefs.GetInt(GameConstants.TOTAL_COINS_KEY, 0);
        }
    }
}