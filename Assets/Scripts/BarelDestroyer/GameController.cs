using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace BarelDestroyer
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private Cannon _cannon;
        [SerializeField] private Menu _menu;
        [SerializeField] private StartScreen _startScreen;
        [SerializeField] private ScreenVisabilityHandler _gameScreen;
        [SerializeField] private BarelSpawner _barelSpawner;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _barelCountText;
        [SerializeField] private EndScreen _endScreen;
        [SerializeField] private GameType _gameType;
        [SerializeField] private AudioSource _hitSound;
        [SerializeField] private AudioSource _endGameSound;
        [SerializeField] private float _spawnInterval = 1.5f;
        [SerializeField] private float _initTimerValue = 60f;

        private float _timer;
        private int _barrelCount;
        private bool _isTimerRunning;
        private bool _isSpawning;

        private GameState _currentGameState;

        private enum GameState
        {
            Starting,
            Playing,
            Paused,
            End
        }

        private void OnEnable()
        {
            _barelSpawner.BarrelDestroyed += ProcessBarrelHit;
            _startScreen.PlayClicked += StartGame;
            _menu.RestartClicked += RestartGame;
            _menu.MenuClicked += OpenStartScreen;
            _menu.ContinueClicked += ContinueGame;
            _endScreen.RestartClicked += RestartGame;
            _endScreen.MainMenuClicked += OpenStartScreen;
        }

        private void OnDisable()
        {
            _barelSpawner.BarrelDestroyed -= ProcessBarrelHit;
            _startScreen.PlayClicked -= StartGame;
            _menu.RestartClicked -= RestartGame;
            _menu.MenuClicked -= OpenStartScreen;
            _menu.ContinueClicked -= ContinueGame;
            _endScreen.RestartClicked -= RestartGame;
            _endScreen.MainMenuClicked -= OpenStartScreen;
        }

        private void Start()
        {
            OpenStartScreen();
        }

        public void OnPauseGame()
        {
            SetGameState(GameState.Paused);
        }

        public void OpenStartScreen()
        {
            ResetAllValues();
            _gameScreen.DisableScreen();
            _cannon.gameObject.SetActive(false);
            _menu.Disable();
            _endScreen.Disable();
            _startScreen.Enable();
        }

        private void StartGame()
        {
            ResetAllValues();
            SetGameState(GameState.Playing);
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

        private void SetGameState(GameState newState)
        {
            _currentGameState = newState;

            switch (_currentGameState)
            {
                case GameState.Playing:
                    EnterPlayingState();
                    break;
                case GameState.Paused:
                    EnterPausedState();
                    break;
                case GameState.End:
                    ProcessGameEnd();
                    break;
                default:
                    ResetAllValues();
                    break;
            }
        }

        private void EnterPlayingState()
        {
            _cannon.gameObject.SetActive(true);
            _gameScreen.EnableScreen();

            if (!_isSpawning)
            {
                StartCoroutine(StartSpawning());
            }

            if (!_isTimerRunning)
            {
                StartCoroutine(TimerCountdown());
            }
        }

        private void EnterPausedState()
        {
            StopAllRunningCoroutines();
            _barelSpawner.ReturnAllObjectsToPool();
            _cannon.gameObject.SetActive(false);
            _menu.Enable();
            _gameScreen.DisableScreen();
        }

        private IEnumerator StartSpawning()
        {
            _isSpawning = true;
            WaitForSeconds interval = new WaitForSeconds(_spawnInterval);

            while (true)
            {
                _barelSpawner.Spawn();
                yield return interval;
            }
        }

        private IEnumerator TimerCountdown()
        {
            _isTimerRunning = true;
            int lastDisplayedTime = Mathf.CeilToInt(_timer);

            while (_timer > 0)
            {
                _timer -= Time.deltaTime;
                int newDisplayedTime = Mathf.CeilToInt(_timer);

                if (newDisplayedTime != lastDisplayedTime)
                {
                    lastDisplayedTime = newDisplayedTime;
                    int minutes = newDisplayedTime / 60;
                    int seconds = newDisplayedTime % 60;
                    _timerText.text = $"{minutes:00}:{seconds:00}";
                }

                yield return null;
            }

            _isTimerRunning = false;
            SetGameState(GameState.End);
        }

        private void ProcessBarrelHit()
        {
            _barrelCount++;
            _hitSound.Play();
            _barelCountText.text = _barrelCount.ToString();
        }

        private void ResetAllValues()
        {
            _barrelCount = 0;
            _timer = _initTimerValue;

            StopAllRunningCoroutines();

            _barelCountText.text = _barrelCount.ToString();
            _timerText.text = "01:00";
            _barelSpawner.ReturnAllObjectsToPool();
            _cannon.ReturnToDefaultPosition();
        }

        private void StopAllRunningCoroutines()
        {
            StopAllCoroutines();
            _isTimerRunning = false;
            _isSpawning = false;
        }

        private void ProcessGameEnd()
        {
            UpdateBestValue();
            
            var statsData = new StatisticsData(
                StatisticsDataHolder.StatisticsDatas[3].GamesPlayed + 1,
                StatisticsDataHolder.StatisticsDatas[3].SuccessfulGames + 1,
                0,
                StatisticsDataHolder.StatisticsDatas[3].CollectedBonuses + _barrelCount,
                StatisticsDataHolder.StatisticsDatas[3].BestTime);

            StatisticsDataHolder.UpdateGameStatistics(_gameType, statsData);
            _endGameSound.Play();
            _endScreen.Enable(_barrelCount, _timerText.text, StatisticsDataHolder.StatisticsDatas[3].BestTime);

            StopAllRunningCoroutines();
            _cannon.gameObject.SetActive(false);
        }
        
        
        private void UpdateBestValue()
        {
            var currentStats = StatisticsDataHolder.StatisticsDatas[3];
            if (currentStats == null) return;

            if (_barrelCount > currentStats.BestTime)
            {
                currentStats.BestTime = _barrelCount;
            }
        }
    }
}
