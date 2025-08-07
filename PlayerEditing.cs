using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEditing : MonoBehaviour
{
    [SerializeField] Players players;
    [SerializeField] Nations nations;
    [SerializeField] PlayersScript playersScript;
    [SerializeField] NationsScript nationsScript;
    // Start is called before the first frame update
    void Start()
    {
        //ResetContexts();
        //ResetStats();
    }

    public int GetOlympicNumber()
    {
        int olympic_num = 0;
        foreach (Player player in players.playerList)
        {
            if (!player.NPC) olympic_num += player.championshipWins;
        }
        return olympic_num;
    }

    public void ResetStats()
    {
        foreach (Player player in players.playerList)
        {
            player.medalPoints = 0;
            player.goldenBoots = 0;
            player.goldMedals = 0;
            player.OlympicGameLost = 0;
            player.OlympicGameWinPercentage = 0;
            player.OlympicGameWon = 0;
            player.OlympicGoalMissed = 0;
            player.OlympicGoalScored = 0;
            player.OlympicGoalScorePercentage = 0;
            player.OlympicPenaltyMissed = 0;
            player.OlympicPenaltyScored = 0;
            player.OlympicPenaltyScorePercentage = 0;
            player.PenaltyShootOutLosesOlympics = 0;
            player.PenaltyShootOutVictoriesOlympics = 0;
            player.PenaltyShootOutWinPercentageOlympics = 0;
            player.silverMedals = 0;
            player.timesQualified = 0;
            player.bronzeMedals = 0;
            player.championshipFinals = 0;
            player.championshipWins = 0;
            player.firstPlaceFinishers = 0;
            player.context = "None";
            player.currentContext = "None";
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
