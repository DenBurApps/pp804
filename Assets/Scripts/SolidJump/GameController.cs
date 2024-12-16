using System.Collections;
using RunningCube;
using TMPro;
using UnityEngine;

namespace SolidJump
{
    public class GameController : MonoBehaviour
    {
        private const float SpawnInterval = 1f;
        
        [SerializeField] private RotationScreen _rotateScreen;
        [SerializeField] private PlayerMover _player;
        [SerializeField] private MovingObject _startPlatform;
        [SerializeField] private GameType _gameType;
        [SerializeField] private EndScreen _endScreen;
        [SerializeField] private Menu _menu;
        [SerializeField] private PlayerBalance _playerBalance;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _fishText;
        [SerializeField] private StartScreen _startScreen;
        [SerializeField] private ScreenVisabilityHandler _ingameElements;
        [SerializeField] private InteractableObjectSpawner _spawner;
        [SerializeField] private PlayerCatcher _playerCatcher;
        [SerializeField] private AudioSource _bgSound;
        [SerializeField] private AudioSource _scoreSound;
        [SerializeField] private AudioSource _endGameSound;
        
        private int _fish;
        private float _timer;
        
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

            _spawner.FishCatched += ProcessFishCatched;

            _playerCatcher.PlayerCathced += ProcessGameEnd;

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

            _spawner.FishCatched -= ProcessFishCatched;

            _playerCatcher.PlayerCathced -= ProcessGameEnd;

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
            _player.DisableInput();
            _menu.Disable();
            _endScreen.Disable();
            _startScreen.Enable();
        }

        private void StartGame()
        {
            _ingameElements.EnableScreen();
            _startScreen.Disable();
            _startPlatform.EnableMovement();

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
                    _player.EnableInput();
                    _ingameElements.EnableScreen();
                    _startPlatform.EnableMovement();

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
                    SetGameState(GameState.Paused);
                    _startPlatform.gameObject.SetActive(true);
                    _startPlatform.ReturnToDefaultPosition();
                    _startPlatform.DisableMovement();

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

        private void ProcessFishCatched()
        {
            _fish += 5;
            _scoreSound.Play();
            _fishText.text = _fish.ToString();
        }
        
        private void ProcessGameEnd()
        {
            UpdateBestTimeValue();
            
            var statsData = new StatisticsData(StatisticsDataHolder.StatisticsDatas[1].GamesPlayed + 1, StatisticsDataHolder.StatisticsDatas[1].SuccessfulGames + 1, 0,
                StatisticsDataHolder.StatisticsDatas[1].CollectedBonuses + _fish,
                StatisticsDataHolder.StatisticsDatas[1].BestTime);

            StatisticsDataHolder.UpdateGameStatistics(_gameType, statsData);
            _endScreen.Enable(_fish, _timerText.text, StatisticsDataHolder.StatisticsDatas[1].BestTime);
            _endGameSound.Play();

            if (_fish > 0)
            {
                _playerBalance.IncreaseBalance(_fish);
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
            _fish = 0;
            _timer = 0;

            if (_spawnCoroutine != null)
                StopCoroutine(_spawnCoroutine);

            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            _spawnCoroutine = null;
            _timerCoroutine = null;
            
            _fishText.text = _fish.ToString();
            
            _timerText.text = Mathf.CeilToInt(_timer).ToString();
            _spawner.ReturnAllObjectsToPool();
            _player.DisableInput();
            _player.ReturnToDefaultPosition();
            _startPlatform.gameObject.SetActive(true);
            _startPlatform.ReturnToDefaultPosition();
        }
        
        private void UpdateBestTimeValue()
        {
            if (StatisticsDataHolder.StatisticsDatas[1].BestTime > 0)
            {
                var bestTime = StatisticsDataHolder.StatisticsDatas[1].BestTime;

                if (_timer > bestTime)
                {
                    StatisticsDataHolder.StatisticsDatas[1].BestTime = _timer;
                }
            }
            else
            {
                StatisticsDataHolder.StatisticsDatas[1].BestTime = _timer;
            }
        }
    }
}
