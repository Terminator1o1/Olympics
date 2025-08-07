using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    [SerializeField] public string name;
    [SerializeField] public string nationality;
    [SerializeField] public int baseGoalScorePercentage = 0;
    [SerializeField] public int basePenaltyScorePercentage = 0;
    [SerializeField] public bool NPC = true;
    [SerializeField] public string rank;
    [SerializeField] public bool Qualifier = false;
    //[SerializeField] public int matchGoals = 0;
    //[SerializeField] public int matchPenalties = 0;
    //[SerializeField] public int matchPenaltyShootOut = 0;
    [SerializeField] public int PenaltyShootOutVictoriesOlympics = 0;
    [SerializeField] public int PenaltyShootOutLosesOlympics = 0;
    [SerializeField] public int PenaltyShootOutWinPercentageOlympics = 0;
    [SerializeField] public int championshipWins = 0;
    [SerializeField] public int championshipFinals = 0;
    [SerializeField] public QualifyingStatus status = QualifyingStatus.NonCandidate;
    [SerializeField] public int goalScorePercentage = 0;
    [SerializeField] public int penaltyScorePercentage = 0;
    [SerializeField] public string context = "Qualifiers";
    [SerializeField] public int firstPlaceFinishers = 0;
    [SerializeField] public int timesQualified = 0;
    [SerializeField] public int OlympicGoalScored = 0;
    [SerializeField] public int OlympicGoalMissed = 0;
    [SerializeField] public int OlympicGoalScorePercentage = 0;
    [SerializeField] public int OlympicPenaltyScored = 0;
    [SerializeField] public int OlympicPenaltyMissed = 0;
    [SerializeField] public int OlympicPenaltyScorePercentage = 0;
    [SerializeField] public int OlympicGameWon = 0;
    [SerializeField] public int OlympicGameLost = 0;
    [SerializeField] public int OlympicGameWinPercentage = 0;
    [SerializeField] public int goldMedals = 0;
    [SerializeField] public int silverMedals = 0;
    [SerializeField] public int bronzeMedals = 0;
    [SerializeField] public int medalPoints = 0;
    [SerializeField] public int goldenBoots = 0;
    [SerializeField] public int currentGoalsScored = 0;
    [SerializeField] public string currentContext;
    [SerializeField] public string[] acheivments = new string[1000];
    [SerializeField] public int rating = 0;
    [SerializeField] public int olympicSeed = 0;
    [SerializeField] public int OlympicsAtNo1 = 0;
    [SerializeField] public int worldCupGoals = 0;
    [SerializeField] public int worldCupGoldenBoots = 0;
}
