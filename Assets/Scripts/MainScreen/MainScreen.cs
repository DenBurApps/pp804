using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class MainScreen : MonoBehaviour
{
    private ScreenVisabilityHandler _screenVisabilityHandler;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    public void EnableScreen()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    public void DisableScreen()
    {
        _screenVisabilityHandler.DisableScreen();
    }
}
