using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatisticsData
{
    public int GamesPlayed;
    public int SuccessfulGames;
    public int FailedGames;
    public int CollectedBonuses;
    public float BestTime;
    public GameType GameType;

    public StatisticsData(int gamesPlayed, int successfulGames, int failedGames, int collectedBonuses, float bestTime)
    {
        GamesPlayed = gamesPlayed;
        SuccessfulGames = successfulGames;
        FailedGames = failedGames;
        CollectedBonuses = collectedBonuses;
        BestTime = bestTime;
    }
}

public enum GameType
{
    RunningCube,
    SolidJump,
    MemoryChallenge,
    BarrelDestroyer,
    QuickHands
}
