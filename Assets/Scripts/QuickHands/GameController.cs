using System;
using System.Collections;
using BarelDestroyer;
using TMPro;
using UnityEngine;

namespace QuickHands
{
    public class GameController : MonoBehaviour
    {
        private const float SpawnInterval = 0.5f;
        private const float InitTimerValue = 60;
        
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private ObjectSpawner _objectSpawner;
        [SerializeField] private GameType _gameType;
        [SerializeField] private BarelDestroyer.EndScreen _endScreen;
        [SerializeField] private Player _player;
        [SerializeField] private StartScreen _startScreen;
        [SerializeField] private ScreenVisabilityHandler _gameScreen;
        [SerializeField] private Menu _menu;
        [SerializeField] private AudioSource _scoreSound;
        [SerializeField] private AudioSource _endGameSound;
        [SerializeField] private AudioSource _hitSound;
        
        private int _score;
        private float _timer;
        
        private IEnumerator _spawnCoroutine;
        private IEnumerator _timerCoroutine;

        private enum GameState
        {
            Starting,
            Playing,
            Paused,
            End,
        }
        
        private GameState _currentGameState;
        
        private void OnEnable()
        {
            _player.EdibleCatched += ProcessEdibleCatched;
            _player.UnedibleCatched += ProcessUnedibleCatched;

            _endScreen.RestartClicked += StartNewGame;
            _endScreen.MainMenuClicked += OpenStartScreen;
            
            _menu.MenuClicked += OpenStartScreen;
            _menu.ContinueClicked += ContinueGame;
            _menu.RestartClicked += StartNewGame;

            _startScreen.PlayClicked += StartNewGame;

        }

        private void OnDisable()
        {
            _player.EdibleCatched -= ProcessEdibleCatched;
            _player.UnedibleCatched -= ProcessUnedibleCatched;

            _endScreen.RestartClicked -= StartNewGame;
            _endScreen.MainMenuClicked -= OpenStartScreen;
            
            _menu.MenuClicked -= OpenStartScreen;
            _menu.ContinueClicked -= ContinueGame;
            _menu.RestartClicked -= StartNewGame;
            
            _startScreen.PlayClicked -= StartNewGame;
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
            _player.StopDetectingTouch();
            _menu.Disable();
            _endScreen.Disable();
            _startScreen.Enable();
        }
        
        private void StartNewGame()
        {
            _endScreen.Disable();
            _menu.Disable();
            ResetAllValues();
            SetGameState(GameState.Playing);
        }
        
        private void ContinueGame()
        {
            SetGameState(GameState.Playing);
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
                    _player.StartDetectingTouch();
                    _gameScreen.EnableScreen();

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

                case GameState.Paused:
                    _objectSpawner.ReturnAllObjectsToPool();
                    _player.StopDetectingTouch();
                    _gameScreen.DisableScreen();
                    _menu.Enable();

                    if (_spawnCoroutine != null)
                        StopCoroutine(_spawnCoroutine);
                    _spawnCoroutine = null;

                    if (_timerCoroutine != null)
                        StopCoroutine(_timerCoroutine);
                    _timerCoroutine = null;
                    break;
                
                case GameState.End:
                    ProcessGameEnd();
                    break;
            }
        }
        
        private IEnumerator StartSpawning()
        {
            WaitForSeconds interval = new WaitForSeconds(SpawnInterval);

            while (true)
            {
                _objectSpawner.Spawn();
                yield return interval;
            }
        }
        
        private IEnumerator TimerCountdown()
        {
            while (_timer > 0)
            {
                _timer -= Time.deltaTime;

                int minutes = Mathf.FloorToInt(_timer / 60);
                int seconds = Mathf.FloorToInt(_timer % 60);

                _timerText.text = $"{minutes:00}:{seconds:00}";

                yield return null;
            }

            _timer = 0;
            _timerText.text = "00:00";
            SetGameState(GameState.End);
        }
        
        private void ProcessEdibleCatched(InteractableObject @object)
        {
            _score++;
            _scoreSound.Play();
            _scoreSound.Play();
            _scoreText.text = _score.ToString();
        }

        private void ProcessUnedibleCatched(InteractableObject @object)
        {
            _score = Mathf.Clamp(_score - 1, 0, int.MaxValue);
            _hitSound.Play();
            _scoreText.text = _score.ToString();
        }
        
        private void ProcessGameEnd()
        {
            UpdateBestValue();
            
            var statsData = new StatisticsData(StatisticsDataHolder.StatisticsDatas[4].GamesPlayed + 1, StatisticsDataHolder.StatisticsDatas[4].SuccessfulGames + 1, 0,
                StatisticsDataHolder.StatisticsDatas[4].CollectedBonuses + _score,
                StatisticsDataHolder.StatisticsDatas[4].BestTime);

            StatisticsDataHolder.UpdateGameStatistics(_gameType, statsData);
            
            if (_spawnCoroutine != null)
                StopCoroutine(_spawnCoroutine);

            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            _player.StopDetectingTouch();
            _endScreen.Enable(_score, _timerText.text, StatisticsDataHolder.StatisticsDatas[4].BestTime);
            _endGameSound.Play();
            _objectSpawner.ReturnAllObjectsToPool();
        }
        
        private void ResetAllValues()
        {
            _score = 0;
            _timer = InitTimerValue;

            if (_spawnCoroutine != null)
                StopCoroutine(_spawnCoroutine);

            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            _player.StopDetectingTouch();
            _spawnCoroutine = null;
            _timerCoroutine = null;

            _scoreText.text = _score.ToString();
            _timerText.text = Mathf.CeilToInt(_timer).ToString();
            _objectSpawner.ReturnAllObjectsToPool();
        }
        
        private void UpdateBestValue()
        {
            var currentStats = StatisticsDataHolder.StatisticsDatas[4];
            if (currentStats == null) return;

            if (_score > currentStats.BestTime)
            {
                currentStats.BestTime = _score;
            }
        }
    }
}
