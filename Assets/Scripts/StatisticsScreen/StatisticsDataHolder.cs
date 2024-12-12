using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Application = UnityEngine.Device.Application;

public static class StatisticsDataHolder
{
    private static string SavePath = Path.Combine(Application.persistentDataPath, "Statistics");

    public static List<StatisticsData> StatisticsDatas = new List<StatisticsData>(5);

    static StatisticsDataHolder()
    {
        for (int i = 0; i < StatisticsDatas.Count; i++)
        {
            StatisticsDatas.Add(new StatisticsData(0, 0, 0, 0, 0));
        }
        LoadStats();
    }

    public static void UpdateGameStatistics(GameType type, StatisticsData data)
    {
        switch (type)
        {
            case GameType.RunningCube:
                StatisticsDatas[0] = data;
                break;
            case GameType.SolidJump:
                StatisticsDatas[1] = data;
                break;
            case GameType.MemoryChallenge:
                StatisticsDatas[2] = data;
                break;
            case GameType.BarrelDestroyer:
                StatisticsDatas[3] = data;
                break;
            case GameType.QuickHands:
                StatisticsDatas[4] = data;
                break;
        }

        SaveStats();
    }

    private static void SaveStats()
    {
        var wrapper = new StatisticsDataWrapper(StatisticsDatas);
        var json = JsonConvert.SerializeObject(wrapper, Formatting.Indented);
        File.WriteAllText(json, SavePath);
        Debug.Log("saved");
    }

    private static void LoadStats()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("no saves");
            return;
        }

        var json = File.ReadAllText(SavePath);
        var statisticsDataWrapper = JsonConvert.DeserializeObject<StatisticsDataWrapper>(json);

        for (int i = 0; i < statisticsDataWrapper.StatisticsDatas.Count; i++)
        {
            StatisticsDatas[i] = statisticsDataWrapper.StatisticsDatas[i];
        }
        
        Debug.Log("loaded");
    }
}

[Serializable]
public class StatisticsDataWrapper
{
    public List<StatisticsData> StatisticsDatas;

    public StatisticsDataWrapper(List<StatisticsData> statisticsDatas)
    {
        StatisticsDatas = statisticsDatas;
    }
}