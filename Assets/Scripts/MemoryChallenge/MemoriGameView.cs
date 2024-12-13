using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class MemoriGameView : MonoBehaviour
{
    [SerializeField] private TMP_Text _countText;
    [SerializeField] private Button _pauseButton;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action PauseClicked;

    public string Timer => _countText.text;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _pauseButton.onClick.AddListener(OnPauseClicked);
    }

    private void OnDisable()
    {
        _pauseButton.onClick.RemoveListener(OnPauseClicked);
    }
    
    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    public void SetTimeValue(string value)
    {
        _countText.text = value;
    }

    private void OnPauseClicked() => PauseClicked?.Invoke();
}
