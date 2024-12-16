using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace BarelDestroyer
{
    public class GameController : MonoBehaviour
    {
        private const float SpawnInterval = 1.5f;
        private const float InitTimerValue = 60f;
        
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

        private float _timer;
        private int _barrelCount;
        private IEnumerator _timerCoroutine;
        private IEnumerator _barelSpawnCoroutine;
        
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
            if (_barelSpawner != null)
                _barelSpawner.BarrelDestroyed -= ProcessBarrelHit;

            if (_startScreen != null)
                _startScreen.PlayClicked -= StartGame;

            if (_menu != null)
            {
                _menu.RestartClicked -= RestartGame;
                _menu.MenuClicked -= OpenStartScreen;
                _menu.ContinueClicked -= ContinueGame;
            }

            if (_endScreen != null)
            {
                _endScreen.RestartClicked -= RestartGame;
                _endScreen.MainMenuClicked -= OpenStartScreen;
            }
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

        private void Start()
        {
            _cannon.gameObject.SetActive(false);
            _gameScreen.DisableScreen();
        }

        private void StartGame()
        {
            _gameScreen.EnableScreen();
            _startScreen.Disable();
            _menu.Disable();
            _endScreen.Disable();

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
                case GameState.Starting:
                    ResetAllValues();
                    break;

                case GameState.Playing:
                    _cannon.gameObject.SetActive(true);
                    _gameScreen.EnableScreen();

                    if (_barelSpawnCoroutine == null)
                    {
                        _barelSpawnCoroutine = StartSpawning();
                        StartCoroutine(_barelSpawnCoroutine);
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
                    _barelSpawner.ReturnAllObjectsToPool();
                    _cannon.gameObject.SetActive(false);
                    _gameScreen.DisableScreen();
                    _menu.Enable();

                    if (_barelSpawnCoroutine != null)
                        StopCoroutine(_barelSpawnCoroutine);
                    _barelSpawnCoroutine = null;

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
                _barelSpawner.Spawn();
                yield return interval;
            }
        }

        private IEnumerator TimerCountdown()
        {
            while (_timer > 0)
            {
                float previousTimer = _timer;
                _timer -= Time.deltaTime;

                int minutes = Mathf.FloorToInt(_timer / 60);
                int seconds = Mathf.FloorToInt(_timer % 60);
                
                if (Mathf.FloorToInt(previousTimer) != Mathf.FloorToInt(_timer))
                {
                    _timerText.text = $"{minutes:00}:{seconds:00}";
                }

                yield return null;
            }

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
            _timer = 0;

            if (_barelSpawnCoroutine != null)
                StopCoroutine(_barelSpawnCoroutine);

            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            _barelSpawnCoroutine = null;
            _timerCoroutine = null;
            
            _barelCountText.text = _barrelCount.ToString();

            _timer = InitTimerValue;
            _timerText.text = Mathf.CeilToInt(InitTimerValue).ToString();
            _barelSpawner.ReturnAllObjectsToPool();
            _cannon.gameObject.SetActive(false);
            _cannon.ReturnToDefaultPosition();
        }
        
        private void ProcessGameEnd()
        {
            UpdateBestValue();
            
            var statsData = new StatisticsData(StatisticsDataHolder.StatisticsDatas[3].GamesPlayed + 1, StatisticsDataHolder.StatisticsDatas[3].SuccessfulGames + 1, 0,
                StatisticsDataHolder.StatisticsDatas[3].CollectedBonuses + _barrelCount,
                StatisticsDataHolder.StatisticsDatas[3].BestTime);

            StatisticsDataHolder.UpdateGameStatistics(_gameType, statsData);
            _endGameSound.Play();
            _endScreen.Enable(_barrelCount, _timerText.text, StatisticsDataHolder.StatisticsDatas[3].CollectedBonuses);
            
            if (_barelSpawnCoroutine != null)
                StopCoroutine(_barelSpawnCoroutine);

            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            _barelSpawnCoroutine = null;
            _timerCoroutine = null;
            _cannon.gameObject.SetActive(false);
        }
        
        private void UpdateBestValue()
        {
            int bestBonuses = StatisticsDataHolder.StatisticsDatas[3].CollectedBonuses;
            StatisticsDataHolder.StatisticsDatas[3].CollectedBonuses = Mathf.Max(bestBonuses, _barrelCount);
        }
    }
}
