using RPG.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersScript : MonoBehaviour , ISaveable
{
    [SerializeField] public Player[] playerList;
    [SerializeField] public Nations nations;
    [SerializeField] public string[] contexts;
    [SerializeField] IntArray[] playerStats;
    [SerializeField] Players players;

    [System.Serializable]
    public class IntArray 
    {
        [SerializeField] public int[] stats;
        [SerializeField] public string[] acheivments;
    }

    public int GetStat(Player player, string stat)
    {
        if (stat == "baseGoalScorePercentage") return player.baseGoalScorePercentage;
        if (stat == "basePenaltyScorePercentage") return player.basePenaltyScorePercentage;
        if (stat == "Goal Accuracy") return player.OlympicGoalScorePercentage;
        if (stat == "Penalty Accuracy") return player.OlympicPenaltyScorePercentage;
        if (stat == "Game Win Percentage") return player.OlympicGameWinPercentage;
        if (stat == "Penalty Win Percentage") return player.PenaltyShootOutWinPercentageOlympics;
        if (stat == "Olympics Qualified") return player.timesQualified;
        if (stat == "Golden Boot Awards") return player.goldenBoots;
        if (stat == "Medal Points") return player.medalPoints;
        if (stat == "Championships Won") return player.championshipWins;
        if (stat == "Championship Finals") return player.championshipFinals;
        if (stat == "Group First Places") return player.firstPlaceFinishers;
        if (stat == "Goals") return player.currentGoalsScored;
        return 0;
    }

    public void UpdateContext(Player player, int num)
    {
        //if (player.context == "" || player.context == "Qualifiers") player.context = "Qualifiers";
        int i = 0;
        while (player.context != contexts[i]) i++;
        if (num > i) player.context = contexts[num];
    }

    public Player[] GetPlayers(int val, string stat, bool allPlayers)
    {
        List<Player> players = new List<Player>();
        foreach (Player player in playerList)
        {
            if (val == GetStat(player, stat) && (allPlayers || !player.NPC)) players.Add(player);
        }
        return players.ToArray();
    }
    //public void IncrementChampionshipMatchGoals(Player player)
    //{
    //    player.matchGoals++;
    //}

    //public void IncrementChampionshipMatchPenalties(Player player)
    //{
    //    player.matchPenalties++;
    //}

    //public void IncrementChampionshipMatchPenaltiesShootOut(Player player)
    //{
    //    player.matchPenaltyShootOut++;
    //}

    //public void IncrementChampionshipMatchPenaltiesShootOutVictories(Player player)
    //{
    //    player.PenaltyShooutOutVictories++;
    //}
    public void IncrementChampionshipWins(Player player)
    {
        player.championshipWins++;
    }

    public void IncrementChampionshipFinals(Player player)
    {
        player.championshipFinals++;
    }

    public void IncrementFirstPlaceFinishers(Player player)
    {
        player.firstPlaceFinishers++;
    }

    public void IncrementTimesQualified(Player player)
    {
        player.timesQualified++;
    }

    public void IncrementOlympicGoalScored(Player player)
    {
        player.OlympicGoalScored++;
        player.currentGoalsScored++;
        UpdatePlayerGoalScorePercentage(player);
    }

    public void IncrementOlympicGoalMissed(Player player)
    {
        player.OlympicGoalMissed++;
        UpdatePlayerGoalScorePercentage(player);
    }

    public void UpdatePlayerGoalScorePercentage(Player player)
    {
        player.OlympicGoalScorePercentage = (int)((((float)player.OlympicGoalScored) / (player.OlympicGoalScored + player.OlympicGoalMissed)) * 100);
    }

    public void IncrementOlympicPenaltyScored(Player player)
    {
        player.OlympicPenaltyScored++;
        UpdatePlayerPenaltyScorePercentage(player);
    }

    public void IncrementOlympicPenaltyMissed(Player player)
    {
        player.OlympicPenaltyMissed++;
        UpdatePlayerPenaltyScorePercentage(player);
    }

    public void UpdatePlayerPenaltyScorePercentage(Player player)
    {
        player.OlympicPenaltyScorePercentage = (int)((((float)player.OlympicPenaltyScored) / (player.OlympicPenaltyScored + player.OlympicPenaltyMissed)) * 100);
    }

    public void IncrementOlympicWin(Player player)
    {
        player.OlympicGameWon++;
        UpdatePlayerOlympicWinPercentage(player);
    }

    public void IncrementOlympicLose(Player player)
    {
        player.OlympicGameLost++;
        UpdatePlayerOlympicWinPercentage(player);
    }

    public void UpdatePlayerOlympicWinPercentage(Player player)
    {
        player.OlympicGameWinPercentage = (int)((((float)player.OlympicGameWon) / (player.OlympicGameWon + player.OlympicGameLost)) * 100);
    }

    public void IncrementOlympicPenaltyShootOutWin(Player player)
    {
        player.PenaltyShootOutVictoriesOlympics++;
        UpdatePlayerOlympicPenaltyShootOutWinPercentage(player);
    }

    public void IncrementOlympicPenaltyShootOutLose(Player player)
    {
        player.PenaltyShootOutLosesOlympics++;
        UpdatePlayerOlympicPenaltyShootOutWinPercentage(player);
    }

    public void UpdatePlayerOlympicPenaltyShootOutWinPercentage(Player player)
    {
        player.PenaltyShootOutWinPercentageOlympics = (int)((((float)player.PenaltyShootOutVictoriesOlympics) / (player.PenaltyShootOutVictoriesOlympics + player.PenaltyShootOutLosesOlympics)) * 100);
    }

    public void IncrementGoldMedals(Player player)
    {
        player.goldMedals++;
        player.medalPoints += 1000000;
        //nations.GetNation(player.nationality).gold++;
        //nations.GetNation(player.nationality).medalPoints += 1000000;
    }

    public void IncrementSilverMedals(Player player)
    {
        player.silverMedals++;
        player.medalPoints += 1000;
        //nations.GetNation(player.nationality).silver++;
        //nations.GetNation(player.nationality).medalPoints += 1000;
    }

    public void IncrementBronzeMedals(Player player)
    {
        player.bronzeMedals++;
        player.medalPoints++;
        //nations.GetNation(player.nationality).bronze++;
        //nations.GetNation(player.nationality).medalPoints++;
    }

    public void IncrementGoldenBoots(Player player)
    {
        player.goldenBoots++;
    }

    public object CaptureState()
    {
        playerList = players.playerList;
        IntArray[] newPlayerstats = new IntArray[playerList.Length];
        for (int i = 0; i < playerList.Length; i++)
        {
            int j = 0;
            if (playerList[i].rank == "First") j = 1;
            else if (playerList[i].rank == "Second") j = 2;
            else if (playerList[i].rank == "Third") j = 3;
            int a;
            for (a = 0; a < contexts.Length; a++)
            {
                if (playerList[i].context == contexts[a]) break;
            }
            if (a == contexts.Length) a--;
            newPlayerstats[i] = new IntArray();
            newPlayerstats[i].stats = new int[25];
            newPlayerstats[i].stats[0] = playerList[i].medalPoints;
            newPlayerstats[i].stats[1] = playerList[i].goldenBoots;
            newPlayerstats[i].stats[2] = playerList[i].goldMedals;
            newPlayerstats[i].stats[3] = playerList[i].OlympicGameLost;
            newPlayerstats[i].stats[4] = playerList[i].OlympicGameWinPercentage;
            newPlayerstats[i].stats[5] = playerList[i].OlympicGameWon;
            newPlayerstats[i].stats[6] = playerList[i].OlympicGoalMissed;
            newPlayerstats[i].stats[7] = playerList[i].OlympicGoalScored;
            newPlayerstats[i].stats[8] = playerList[i].OlympicGoalScorePercentage;
            newPlayerstats[i].stats[9] = playerList[i].OlympicPenaltyMissed;
            newPlayerstats[i].stats[10] = playerList[i].OlympicPenaltyScored;
            newPlayerstats[i].stats[11] = playerList[i].OlympicPenaltyScorePercentage;
            newPlayerstats[i].stats[12] = playerList[i].PenaltyShootOutLosesOlympics;
            newPlayerstats[i].stats[13] = playerList[i].PenaltyShootOutVictoriesOlympics;
            newPlayerstats[i].stats[14] = playerList[i].PenaltyShootOutWinPercentageOlympics;
            newPlayerstats[i].stats[15] = playerList[i].silverMedals;
            newPlayerstats[i].stats[16] = playerList[i].timesQualified;
            newPlayerstats[i].stats[17] = playerList[i].bronzeMedals;
            newPlayerstats[i].stats[18] = playerList[i].championshipFinals;
            newPlayerstats[i].stats[19] = playerList[i].championshipWins;
            newPlayerstats[i].stats[20] = playerList[i].firstPlaceFinishers;
            newPlayerstats[i].stats[21] = j;
            newPlayerstats[i].stats[22] = a;
            newPlayerstats[i].stats[23] = playerList[i].OlympicsAtNo1;
            newPlayerstats[i].stats[24] = playerList[i].worldCupGoldenBoots;
            newPlayerstats[i].acheivments = playerList[i].acheivments;
            newPlayerstats[i].stats[0] = playerList[i].medalPoints;
        }
        return newPlayerstats;
    }

    public void RestoreState(object state)
    {
        IntArray[] newPlayerstats = (IntArray[]) state;
        for (int i = 0; i < newPlayerstats.Length; i++)
        {
            playerList[i].medalPoints = newPlayerstats[i].stats[0];
            playerList[i].goldenBoots = newPlayerstats[i].stats[1];
            playerList[i].goldMedals = newPlayerstats[i].stats[2];
            playerList[i].OlympicGameLost = newPlayerstats[i].stats[3];
            playerList[i].OlympicGameWinPercentage = newPlayerstats[i].stats[4];
            playerList[i].OlympicGameWon = newPlayerstats[i].stats[5];
            playerList[i].OlympicGoalMissed = newPlayerstats[i].stats[6];
            playerList[i].OlympicGoalScored = newPlayerstats[i].stats[7];
            playerList[i].OlympicGoalScorePercentage = newPlayerstats[i].stats[8];
            playerList[i].OlympicPenaltyMissed = newPlayerstats[i].stats[9];
            playerList[i].OlympicPenaltyScored = newPlayerstats[i].stats[10];
            playerList[i].OlympicPenaltyScorePercentage = newPlayerstats[i].stats[11];
            playerList[i].PenaltyShootOutLosesOlympics = newPlayerstats[i].stats[12];
            playerList[i].PenaltyShootOutVictoriesOlympics = newPlayerstats[i].stats[13];
            playerList[i].PenaltyShootOutWinPercentageOlympics = newPlayerstats[i].stats[14];
            playerList[i].silverMedals = newPlayerstats[i].stats[15];
            playerList[i].timesQualified = newPlayerstats[i].stats[16];
            playerList[i].bronzeMedals = newPlayerstats[i].stats[17];
            playerList[i].championshipFinals = newPlayerstats[i].stats[18];
            playerList[i].championshipWins = newPlayerstats[i].stats[19];
            playerList[i].firstPlaceFinishers = newPlayerstats[i].stats[20];
            int j = newPlayerstats[i].stats[21];
            if (j == 1) playerList[i].rank = "First";
            else if (j == 2) playerList[i].rank = "Second";
            else if (j == 3) playerList[i].rank = "Third";
            else playerList[i].rank = "";
            playerList[i].context = contexts[newPlayerstats[i].stats[22]];
            playerList[i].OlympicsAtNo1 = newPlayerstats[i].stats[23];
            playerList[i].worldCupGoldenBoots = newPlayerstats[i].stats[24];
            playerList[i].acheivments = newPlayerstats[i].acheivments;                     
            //players.UpdateContexts();
        }
        players.playerList = playerList;
        UpdateNationsStats();
        int olympicNumber = 0;
        foreach(Player player in playerList)
        {
            if (player.nationality == "Israel") olympicNumber += player.championshipWins;
        }
        print(olympicNumber);
        players.UpdateRatings(false);
    }

    private void UpdateNationsStats()
    {
        foreach (Nation nation in nations.nationList)
        {
            nation.medalPoints = 0;
            nation.gold = 0;
            nation.silver = 0;
            nation.bronze = 0;
        }
        foreach (Player player in players.playerList)
        {
            Nation nation = nations.GetNation(player.nationality);
            nation.medalPoints += player.medalPoints;
            nation.gold += player.goldMedals;
            nation.silver += player.silverMedals;
            nation.bronze += player.bronzeMedals;
        }
    }

    //public object CaptureState()
    //{
    //    return playerList;
    //}

    //public void RestoreState(object state)
    //{
    //    playerList = (Player[]) state;
    //}
}
