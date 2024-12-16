using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class MainScreen : MonoBehaviour
{
    private ScreenVisabilityHandler _screenVisabilityHandler;

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    public void OpenRunningCube()
    {
        SceneManager.LoadScene("RunningCubeScene");
    }

    public void OpenSolidJump()
    {
        SceneManager.LoadScene("SolidJumpScene");
    }

    public void OpenMemory()
    {
        SceneManager.LoadScene("MemoryChallengeScene");
    }

    public void OpenBarrel()
    {
        SceneManager.LoadScene("BarrelDestroyerScene");
    }

    public void OpenQuickHands()
    {
        SceneManager.LoadScene("QuickHandsScene");
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
