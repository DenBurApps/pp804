using System;
using System.Collections;
using SpaceMission;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RunningCube
{
    public class GameController : MonoBehaviour
    {
        private const string BestTimeKey = "RunningCubeBestTime";
        private const int SpawnInterval = 1;

        [SerializeField] private Sprite _emptyHeartSprite;
        [SerializeField] private Sprite _fullHeartSprite;

        [SerializeField] private RotationScreen _rotateScreen;
        [SerializeField] private Player _player;
        [SerializeField] private GameType _gameType;
        [SerializeField] private EndScreen _endScreen;
        [SerializeField] private Menu _menu;
        [SerializeField] private PlayerBalance _playerBalance;
        [SerializeField] private Image[] _hearts;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _coinsText;
        [SerializeField] private StartScreen _startScreen;
        [SerializeField] private ScreenVisabilityHandler _ingameElements;
        [SerializeField] private InteractableObjectSpawner _spawner;

        private int _coins;
        private float _timer;
        private int _healthCount;
        
        private IEnumerator _spawnCoroutine;
        private IEnumerator _timerCoroutine;
        
        private GameState _currentGameState;
        
        private enum GameState
        {
            Starting,
            Playing,
            Paused,
            End,
        }

        private void OnEnable()
        {
            _rotateScreen.ScreenClosed += OpenStartScreen;
            
            _spawner.SpikesCatched += ProcessSpikesCollision;
            _spawner.CoinCatched += ProcessCoinCollision;
        }

        private void OnDisable()
        {
            _rotateScreen.ScreenClosed -= OpenStartScreen;
            
            _spawner.SpikesCatched -= ProcessSpikesCollision;
            _spawner.CoinCatched -= ProcessCoinCollision;
        }

        private void OpenStartScreen()
        {
            _ingameElements.DisableScreen();
            _startScreen.Enable();
        }

        private void StartGame()
        {
            _ingameElements.EnableScreen();
            _startScreen.Enable();
            _menu.Disable();
            _endScreen.Disable();
            
            SetGameState(GameState.Playing);
        }
        
        private void UpdateHearts()
        {
            for (int i = 0; i < _hearts.Length; i++)
            {
                _hearts[i].sprite = i < _healthCount ? _fullHeartSprite : _emptyHeartSprite;
            }
        }
        
        private void SetGameState(GameState newState)
        {
            _currentGameState = newState;

            switch (_currentGameState)
            {
                case GameState.Starting:
                    ResetAllValues();
                    break;

                case GameState.Playing:
                    ResetAllValues();

                    if (_spawnCoroutine == null)
                    {
                        _spawnCoroutine = StartSpawning();
                        StartCoroutine(_spawnCoroutine);
                    }

                    if (_timerCoroutine == null)
                    {
                        _timerCoroutine = TimerCountdown();
                        StartCoroutine(_timerCoroutine);
                    }

                    break;

                case GameState.End:
                 //   ProcessGameEnd();
                    break;
            }
        }
        
        private IEnumerator StartSpawning()
        {
            WaitForSeconds interval = new WaitForSeconds(SpawnInterval);

            while (true)
            {
                _spawner.Spawn();
                yield return interval;
            }
        }

        private IEnumerator TimerCountdown()
        {
            _timer = 0;
            _timerText.text = "00:00";

            while (true)
            {
                _timer += Time.deltaTime;

                int minutes = Mathf.FloorToInt(_timer / 60);
                int seconds = Mathf.FloorToInt(_timer % 60);

                _timerText.text = $"{minutes:00}:{seconds:00}";

                yield return null;
            }
        }

        private void ProcessSpikesCollision()
        {
            _healthCount--;
            
            UpdateHearts();

            if (_healthCount <= 0)
            {
                //ProcessGameEnd;
                _endScreen.Enable(_coins, _timerText.text, StatisticsDataHolder.StatisticsDatas[0].BestTime);
            }
        }

        private void ProcessCoinCollision()
        {
            _coins += 5;
            _coinsText.text = _coins.ToString();
        }
        
        private void ResetAllValues()
        {
            _coins = 0;
            _timer = 0;
            _healthCount = 3;

            if (_spawnCoroutine != null)
                StopCoroutine(_spawnCoroutine);

            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            _spawnCoroutine = null;
            _timerCoroutine = null;

            UpdateHearts();
            _coinsText.text = "<sprite name=\"Fra1me 8 2\">  " + _coins.ToString();
            _timerText.text = Mathf.CeilToInt(_timer).ToString();
            _spawner.ReturnAllObjectsToPool();
        }

        private void UpdateBestTimeValue()
        {
            if (PlayerPrefs.HasKey(BestTimeKey))
            {
                var bestTime = PlayerPrefs.GetFloat(BestTimeKey);

                if (_timer > bestTime)
                {
                    PlayerPrefs.SetFloat(BestTimeKey, _timer);
                }
            }
            else
            {
                PlayerPrefs.SetFloat(BestTimeKey, _timer);
            }

            PlayerPrefs.Save();
        }
        
    }
}