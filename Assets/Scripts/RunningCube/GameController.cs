using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RunningCube
{
    public class GameController : MonoBehaviour
    {
        private const float SpawnInterval = 1.5f;

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
        [SerializeField] private AudioSource _scoreSound;
        [SerializeField] private AudioSource _endGameSound;
        [SerializeField] private AudioSource _bgSound;
        [SerializeField] private AudioSource _hitSound;

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

            _startScreen.PlayClicked += StartGame;

            _menu.RestartClicked += RestartGame;
            _menu.MenuClicked += OpenStartScreen;
            _menu.ContinueClicked += ContinueGame;

            _endScreen.RestartClicked += RestartGame;
            _endScreen.MainMenuClicked += OpenStartScreen;
        }

        private void OnDisable()
        {
            _rotateScreen.ScreenClosed -= OpenStartScreen;

            _spawner.SpikesCatched -= ProcessSpikesCollision;
            _spawner.CoinCatched -= ProcessCoinCollision;

            _startScreen.PlayClicked -= StartGame;
            
            _menu.RestartClicked -= RestartGame;
            _menu.MenuClicked -= OpenStartScreen;
            _menu.ContinueClicked -= ContinueGame;

            _endScreen.RestartClicked -= RestartGame;
            _endScreen.MainMenuClicked -= OpenStartScreen;
        }

        public void OnPauseGame()
        {
            SetGameState(GameState.Paused);
        }

        public void OpenStartScreen()
        {
            _bgSound.Play();
            ResetAllValues();
            _ingameElements.DisableScreen();
            _menu.Disable();
            _endScreen.Disable();
            _startScreen.Enable();
        }

        private void StartGame()
        {
            _ingameElements.EnableScreen();
            _startScreen.Disable();

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
                    _player.EnableInput();
                    _ingameElements.EnableScreen();

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
                    ProcessGameEnd();
                    break;

                case GameState.Paused:
                    _spawner.ReturnAllObjectsToPool();
                    _player.DisableInput();
                    _ingameElements.DisableScreen();

                    if (_spawnCoroutine != null)
                        StopCoroutine(_spawnCoroutine);

                    _spawnCoroutine = null;

                    if (_timerCoroutine != null)
                        StopCoroutine(_timerCoroutine);

                    _timerCoroutine = null;
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
            _hitSound.Play();

            UpdateHearts();

            if (_healthCount <= 0)
            {
                UpdateBestTimeValue();
                SetGameState(GameState.End);
            }
        }

        private void ContinueGame()
        {
            SetGameState(GameState.Playing);
        }

        private void RestartGame()
        {
            ResetAllValues();
            SetGameState(GameState.Playing);
        }

        private void ProcessCoinCollision()
        {
            _coins += 5;
            _scoreSound.Play();
            _coinsText.text = _coinsText.text = "<sprite name=\"Fra1me 8 2\">  " + _coins.ToString();
        }

        private void ProcessGameEnd()
        {
            _spawner.ReturnAllObjectsToPool();
            
            var statsData = new StatisticsData(StatisticsDataHolder.StatisticsDatas[0].GamesPlayed + 1, 0, 0,
                StatisticsDataHolder.StatisticsDatas[0].CollectedBonuses + _coins,
                StatisticsDataHolder.StatisticsDatas[0].BestTime);

            StatisticsDataHolder.UpdateGameStatistics(_gameType, statsData);
            _endScreen.Enable(_coins, _timerText.text, StatisticsDataHolder.StatisticsDatas[0].BestTime);
            _endGameSound.Play();

            if (_coins > 0)
            {
                _playerBalance.IncreaseBalance(_coins);
            }
            
            if (_spawnCoroutine != null)
                StopCoroutine(_spawnCoroutine);

            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            _spawnCoroutine = null;
            _timerCoroutine = null;
            _player.DisableInput();
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
            _player.DisableInput();
        }

        private void UpdateBestTimeValue()
        {
            if (StatisticsDataHolder.StatisticsDatas[0].BestTime > 0)
            {
                var bestTime = StatisticsDataHolder.StatisticsDatas[0].BestTime;

                if (_timer > bestTime)
                {
                    StatisticsDataHolder.StatisticsDatas[0].BestTime = _timer;
                }
            }
            else
            {
                StatisticsDataHolder.StatisticsDatas[0].BestTime = _timer;
            }
        }
    }
}