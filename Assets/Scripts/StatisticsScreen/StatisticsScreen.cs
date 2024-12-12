using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class StatisticsScreen : MonoBehaviour
{
    [SerializeField] private StatisticsPlane[] _statisticsPlanes;
    
    private ScreenVisabilityHandler _screenVisabilityHandler;

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

        for (int i = 0; i < StatisticsDataHolder.StatisticsDatas.Count; i++)
        {
            _statisticsPlanes[i].SetData(StatisticsDataHolder.StatisticsDatas[i]);
        }
        
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }
}
