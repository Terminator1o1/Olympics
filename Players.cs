//using RPG.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Players", menuName = "Players/New Players", order = 0)]
public class Players : ScriptableObject
{
    [SerializeField] public Player[] playerList = null;
    [SerializeField] public string[] contexts = null;
    [SerializeField] Nations nations;
    [SerializeField] public string[] contexts_shorts = null;
    [SerializeField] public int[] points = null;

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
        if (stat == "Ratings") return player.rating;
        if (stat == "Olympics at number 1") return player.OlympicsAtNo1;
        if (stat == "Individual Goals") return player.worldCupGoals;
        if (stat == "Champions League Golden Boots") return player.worldCupGoldenBoots;
        return 0;
    }

    public void UpdateContexts()
    {
        int olympic_num = GetOlympicNumber();
        foreach (Player player in playerList)
        {
            int i;
            for (i = 0; i < contexts.Length; i++)
            {
                if (player.context == contexts[i]) break;
            }
            int j;
            for (j = 0; j < contexts.Length; j++)
            {
                if (player.currentContext == contexts[j]) break;
            }
            if (j > i) player.context = player.currentContext;
            player.acheivments[olympic_num] = contexts_shorts[j];
        } 
    }

    public int GetOlympicNumber()
    {
        int olympic_num = 0;
        foreach (Player player in playerList)
        {
            if (!player.NPC) olympic_num += player.championshipWins;
        }
        return olympic_num;
    }

    public void UpdateRatings(bool olympics)
    {
        int olympic_num = GetOlympicNumber();
        Player no1 = null;
        foreach (Nation nation in nations.nationList)
        {
            nation.rating = 0;
        }
        foreach (Player player in playerList)
        {
            player.currentContext = "None";
            player.rating = 0;
            for (int j = olympic_num; j > Mathf.Max(0,olympic_num - 5); j--)
            {
                int i;
                for (i = 0; i < contexts_shorts.Length; i++)
                {
                    if (player.acheivments[j] == contexts_shorts[i]) break;
                }
                player.rating += points[i];
            }
            nations.GetNation(player.nationality).rating += player.rating;
            if (no1 == null) no1 = player;
            else if (player.rating > no1.rating) no1 = player;
        }
        if (olympics) no1.OlympicsAtNo1++;
        foreach (Nation nation in nations.nationList)
        {
            if (nation.numQualifiers == 1)
            {
                nation.rating = nation.rating / 3;
            }
            else if (nation.numStrongCandidate == 1)
            {
                nation.rating = nation.rating / 2;
            }
        }
    }

    public Player[] GetPlayers(int val, string stat, bool allPlayers)
    {
        List<Player> players = new List<Player>();
        foreach (Player player in playerList)
        {
            if (val == GetStat(player, stat) && (stat == "Goals" || (allPlayers || !player.NPC) && ((player.timesQualified >= 5 || stat == "Medal Points" || stat == "baseGoalScorePercentage" || stat == "Ratings" || stat == "Olympics at number 1" || stat == "Golden Boot Awards" || stat == "Olympics Qualified" || stat == "Championships Won" || stat == "Championship Finals" || stat == "Group First Places") && (stat != "Penalty Accuracy" || player.OlympicPenaltyScored >= 15) && (stat != "Penalty Win Percentage" || player.PenaltyShootOutVictoriesOlympics >= 5)))) players.Add(player);
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
        player.OlympicGoalScorePercentage = (int) ((((float) player.OlympicGoalScored) / (player.OlympicGoalScored + player.OlympicGoalMissed)) * 100);
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
        nations.GetNation(player.nationality).gold++;
        nations.GetNation(player.nationality).medalPoints += 1000000;
    }

    public void IncrementSilverMedals(Player player)
    {
        player.silverMedals++;
        player.medalPoints += 1000;
        nations.GetNation(player.nationality).silver++;
        nations.GetNation(player.nationality).medalPoints += 1000;
    }

    public void IncrementBronzeMedals(Player player)
    {
        player.bronzeMedals++;
        player.medalPoints++;
        nations.GetNation(player.nationality).bronze++;
        nations.GetNation(player.nationality).medalPoints++;
    }

    public void IncrementGoldenBoots(Player player)
    {
        player.goldenBoots++;
    }

    //public object CaptureState()
    //{
    //    //playerList[0].context = "bruh";
    //    return playerList;
    //}

    //public void RestoreState(object state)
    //{
    //    playerList = (Player[]) state;
    //    //playerList[0].context = "bruh";
    //}
}
