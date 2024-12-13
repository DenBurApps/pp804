using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class EndScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text _coinsText;
    [SerializeField] private TMP_Text _currentTimeText;
    [SerializeField] private TMP_Text _bestTimeText;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action RestartClicked;
    public event Action MainMenuClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void Start()
    {
        Disable();
    }

    public void Enable(int coins, string time, float bestTime)
    {
        _screenVisabilityHandler.EnableScreen();

        if (_coinsText != null)
            _coinsText.text = "<sprite name=\"Fra1me 8 2\">  " + coins.ToString();
        
        _currentTimeText.text = time;

        int minutes = Mathf.FloorToInt(bestTime / 60);
        int seconds = Mathf.FloorToInt(bestTime % 60);

        _bestTimeText.text = $"{minutes:00}:{seconds:00}";
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    public void OnRestartClicked()
    {
        RestartClicked?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }

    public void OnMainMenuClicked()
    {
        MainMenuClicked?.Invoke();
        Disable();
    }
}