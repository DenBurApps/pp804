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

        private void Awake()
        {
            AssertSerializedFields();
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void AssertSerializedFields()
        {
            Debug.Assert(_rotateScreen != null, "RotateScreen is not assigned!");
            Debug.Assert(_player != null, "PlayerMover is not assigned!");
            Debug.Assert(_startPlatform != null, "StartPlatform is not assigned!");
            Debug.Assert(_endScreen != null, "EndScreen is not assigned!");
            Debug.Assert(_menu != null, "Menu is not assigned!");
            Debug.Assert(_playerBalance != null, "PlayerBalance is not assigned!");
            Debug.Assert(_timerText != null, "TimerText is not assigned!");
            Debug.Assert(_fishText != null, "FishText is not assigned!");
            Debug.Assert(_startScreen != null, "StartScreen is not assigned!");
            Debug.Assert(_ingameElements != null, "IngameElements is not assigned!");
            Debug.Assert(_spawner != null, "Spawner is not assigned!");
            Debug.Assert(_playerCatcher != null, "PlayerCatcher is not assigned!");
        }

        private void SubscribeToEvents()
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

        private void UnsubscribeFromEvents()
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
            _startScreen.Disable();
            _startPlatform.EnableMovement();
            SetGameState(GameState.Playing);
        }

        private void ContinueGame()
        {
            _player.gameObject.SetActive(true);
            _player.ReturnToDefaultPosition();
            _player.EnableInput();
            _startPlatform.gameObject.SetActive(true);
            _startPlatform.ReturnToDefaultPosition();
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

                    StartCoroutineIfNotRunning(ref _spawnCoroutine, StartSpawning());
                    StartCoroutineIfNotRunning(ref _timerCoroutine, TimerCountdown());
                    break;

                case GameState.End:
                    ProcessGameEnd();
                    break;

                case GameState.Paused:
                    PauseGameLogic();
                    break;
            }
        }

        private void StartCoroutineIfNotRunning(ref IEnumerator coroutine, IEnumerator routine)
        {
            if (coroutine == null)
            {
                coroutine = routine;
                StartCoroutine(coroutine);
            }
        }

        private void StopCoroutineIfRunning(ref IEnumerator coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        private void PauseGameLogic()
        {
            _spawner.ReturnAllObjectsToPool();
            _player.DisableInput();
            _player.gameObject.SetActive(false);
            _ingameElements.DisableScreen();
            _startPlatform.DisableMovement();
            StopCoroutineIfRunning(ref _spawnCoroutine);
            StopCoroutineIfRunning(ref _timerCoroutine);
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
            UpdateUI("00:00", _fish);

            WaitForSeconds updateInterval = new WaitForSeconds(0.1f);

            while (true)
            {
                _timer += 0.1f;
                int minutes = Mathf.FloorToInt(_timer / 60);
                int seconds = Mathf.FloorToInt(_timer % 60);
                UpdateUI($"{minutes:00}:{seconds:00}", _fish);
                yield return updateInterval;
            }
        }

        private void ProcessFishCatched()
        {
            _fish += 5;
            _scoreSound.Play();
            UpdateUI(_timerText.text, _fish);
        }

        private void ProcessGameEnd()
        {
            UpdateBestTimeValue();

            var statsData = new StatisticsData(
                StatisticsDataHolder.StatisticsDatas[1].GamesPlayed + 1,
                StatisticsDataHolder.StatisticsDatas[1].SuccessfulGames + 1,
                0,
                StatisticsDataHolder.StatisticsDatas[1].CollectedBonuses + _fish,
                StatisticsDataHolder.StatisticsDatas[1].BestTime
            );

            StatisticsDataHolder.UpdateGameStatistics(_gameType, statsData);
            _endScreen.Enable(_fish, _timerText.text, StatisticsDataHolder.StatisticsDatas[1].BestTime);
            _endGameSound.Play();

            if (_fish > 0)
            {
                _playerBalance.IncreaseBalance(_fish);
            }

            StopCoroutineIfRunning(ref _spawnCoroutine);
            StopCoroutineIfRunning(ref _timerCoroutine);
            _player.DisableInput();
        }

        private void ResetAllValues()
        {
            _fish = 0;
            _timer = 0;

            StopCoroutineIfRunning(ref _spawnCoroutine);
            StopCoroutineIfRunning(ref _timerCoroutine);

            UpdateUI("00:00", _fish);

            _spawner.ReturnAllObjectsToPool();
            _player.gameObject.SetActive(true);
            _player.DisableInput();
            _player.ReturnToDefaultPosition();
            _startPlatform.gameObject.SetActive(true);
            _startPlatform.ReturnToDefaultPosition();
        }

        private void UpdateBestTimeValue()
        {
            var currentStats = StatisticsDataHolder.StatisticsDatas[1];
            if (currentStats == null) return;

            if (_timer > currentStats.BestTime)
            {
                currentStats.BestTime = _timer;
            }
        }

        private void UpdateUI(string timer, int fish)
        {
            _timerText.text = timer;
            _fishText.text = fish.ToString();
        }
    }
}
