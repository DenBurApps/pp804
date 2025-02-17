using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatisticsPlane : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _gamesPlayed, _collectedBonuses, _bestTime, _successfulGames, _failedGames, _bestValue;

    public void SetData(StatisticsData data)
    {
        _gamesPlayed.text = data.GamesPlayed.ToString();

        if (_collectedBonuses != null)
            _collectedBonuses.text = data.CollectedBonuses.ToString();

        if (_bestTime != null)
        {
            int minutes = Mathf.FloorToInt(data.BestTime / 60);
            int seconds = Mathf.FloorToInt(data.BestTime % 60);
            _bestTime.text = $"{minutes:00}:{seconds:00}";
        }
        
        if (_bestValue != null)
            _bestValue.text = data.BestTime.ToString();

        if (_successfulGames != null)
            _successfulGames.text = data.SuccessfulGames.ToString();

        if (_failedGames != null)
            _failedGames.text = data.FailedGames.ToString();
    }
}