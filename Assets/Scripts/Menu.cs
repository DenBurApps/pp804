using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class Menu : MonoBehaviour
{
    private ScreenVisabilityHandler _screenVisabilityHandler;
    
    public event Action ContinueClicked;

    public event Action RestartClicked;
    public event Action MenuClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void Start()
    {
        Disable();
    }

    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    public void OnContinueClicked()
    {
        ContinueClicked?.Invoke();
        Disable();
    }

    public void OnRestartClicked()
    {
        RestartClicked?.Invoke();
        Disable();
    }

    public void GoToMainMenu()
    {
        MenuClicked?.Invoke();
        Disable();
    }
}
