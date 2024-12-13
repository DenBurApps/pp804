using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MemoryChallenge
{
    public class MemoryGameController : MonoBehaviour
    {
        [SerializeField] private CellHolder _cellHolder;
        [SerializeField] private EndScreen _endScreen;
        [SerializeField] private Menu _menu;
        [SerializeField] private MemoriGameView _view;
        [SerializeField] private CellSpriteProvider _cellSpriteProvider;
        [SerializeField] private StartScreen _startScreen;
        [SerializeField] private ScreenVisabilityHandler _ingameElements;
        [SerializeField] private TMP_Text _collectedPairsText;
        [SerializeField] private GameType _gameType;

        private CellTypeProvider _cellTypeProvider;
        private float _currentTime;
        private IEnumerator _timerCoroutine;
        private Cell _firstCell;
        private Cell _secondCell;
        private int _cellPairs;

        private void Awake()
        {
            _cellTypeProvider = new CellTypeProvider();
            _cellPairs = _cellHolder.Cells.Count / 2;
        }

        private void OnEnable()
        {
            _view.PauseClicked += PauseGame;

            foreach (var cell in _cellHolder.Cells)
            {
                cell.SetCellSpriteProvider(_cellSpriteProvider);
                cell.Clicked += ProcessCellClicked;
            }

            _startScreen.PlayClicked += ProcessNewGameStart;
            
            _menu.RestartClicked += ProcessNewGameStart;
            _menu.MenuClicked += OpenStartScreen;
            _menu.ContinueClicked += ContinueGame;
            
            _endScreen.RestartClicked += ProcessNewGameStart;
            _endScreen.MainMenuClicked += OpenStartScreen;
        }

        private void OnDisable()
        {
            foreach (var cell in _cellHolder.Cells)
            {
                cell.Clicked -= ProcessCellClicked;
            }
            
            _startScreen.PlayClicked -= ProcessNewGameStart;

            _endScreen.RestartClicked -= ProcessNewGameStart;
            _endScreen.MainMenuClicked -= OpenStartScreen;
            _menu.RestartClicked -= ProcessNewGameStart;
            _menu.MenuClicked -= OpenStartScreen;
            _menu.ContinueClicked -= ContinueGame;
            _view.PauseClicked -= PauseGame;
        }

        private void Start()
        {
            _view.Disable();
        }

        private void ProcessNewGameStart()
        {
            _view.Enable();
            _ingameElements.EnableScreen();
            _startScreen.Disable();
            ResetDefaultValues();

            _endScreen.Disable();
            _menu.Disable();
            _timerCoroutine = StartTimer();
            StartCoroutine(_timerCoroutine);
        
            List<CellTypes> cellTypesList =  _cellTypeProvider.GetPair(_cellPairs);

            for (int i = 0; i < cellTypesList.Count; i++)
            {
                _cellHolder.Cells[i].ReturnToDefault();
                _cellHolder.Cells[i].SetCellType(cellTypesList[i]);
            }
        }

        private void ProcessCellClicked(Cell cell)
        {
            if (cell.IsFliped)
                return;

            if (_firstCell != null && _secondCell != null)
            {
                _firstCell.HideCellImage();
                _secondCell.HideCellImage();

                _firstCell = null;
                _secondCell = null;
            }

            if (_firstCell == null)
            {
                _firstCell = cell;
                _firstCell.ShowCellImage();
                return;
            }

            if (_secondCell == null && _firstCell != cell)
            {
                _secondCell = cell;
                _secondCell.ShowCellImage();
            }

            if (_firstCell != null && _secondCell != null)
                CompareChosenCells();
        }

        private void CompareChosenCells()
        {
            if (_firstCell.CurrentType == _secondCell.CurrentType)
            {
                _cellPairs--;

                _firstCell.Disable();
                _secondCell.Disable();

                if (_cellPairs <= 0)
                {
                    ProcessGameWon();
                }

                _firstCell = null;
                _secondCell = null;
                UpdatePairsText();
            }
        }

        private void ResetDefaultValues()
        {
            _cellPairs = _cellHolder.Cells.Count / 2;
            _currentTime = 0;
            UpdateTimer(_currentTime);

            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            _timerCoroutine = null;

            _firstCell = null;
            _secondCell = null;
            UpdatePairsText();
        }

        private IEnumerator StartTimer()
        {
            _currentTime = 0;

            while (_currentTime >= 0f)
            {
                _currentTime += Time.deltaTime;
                UpdateTimer(_currentTime);
                yield return null;
            }
        }

        private void UpdateTimer(float time)
        {
            time += Time.deltaTime;

            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);

            _view.SetTimeValue($"{minutes:00}:{seconds:00}"); 
        }

        private void UpdatePairsText()
        {
            _collectedPairsText.text = $"{_cellPairs}/{_cellHolder.Cells.Count / 2}";
        }
        
        private void ProcessGameWon()
        {
            var statsData = new StatisticsData(StatisticsDataHolder.StatisticsDatas[2].GamesPlayed++,StatisticsDataHolder.StatisticsDatas[2].SuccessfulGames++ , 0,
                StatisticsDataHolder.StatisticsDatas[2].CollectedBonuses + 0,
                StatisticsDataHolder.StatisticsDatas[2].BestTime);

            StatisticsDataHolder.UpdateGameStatistics(_gameType, statsData);
            _endScreen.Enable(0, _view.Timer, StatisticsDataHolder.StatisticsDatas[2].BestTime);
            
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
        }
        public void OpenStartScreen()
        {
            ResetDefaultValues();
            _ingameElements.DisableScreen();
            _menu.Disable();
            _endScreen.Disable();
            _startScreen.Enable();
            _view.Disable();
        }
        
        private void PauseGame()
        {
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
            
            _menu.Enable();
        }

        private void ContinueGame()
        {
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
            }

            _timerCoroutine = StartTimer();
            StartCoroutine(_timerCoroutine);
            _view.Enable();
        }
    }
}