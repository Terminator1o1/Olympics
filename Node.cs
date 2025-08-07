using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Node
{
    [SerializeField] public Player player1;
    [SerializeField] public Player player2;
    [SerializeField] public Node nextNode;
    [SerializeField] public Match match;
    [SerializeField] public Qualifiers qualifiers;
    [SerializeField] public Game game;
    [SerializeField] public Olympics olympics;
    [SerializeField] public Nations nations;
    [SerializeField] public Players players;
    private Soundtrack soundtrack;
    [SerializeField] List<Player> victors;
    [SerializeField] Text[] texts;
    [SerializeField] RawImage[] flags;
    int round;
    public WorldCupGroup worldCupGroup;
    public WorldCupDraw worldCupDraw;

    public Node(Player player1, Player player2, Node nextNode, Match match, Qualifiers qualifiers, Game game, Olympics olympics,Nations nations, Text[] texts, RawImage[] flags, int round, Players players, int seed1, int seed2, Soundtrack soundtrack, WorldCupGroup worldCupGroup, WorldCupDraw worldCupDraw)
    {
        this.player1 = player1;
        this.player2 = player2;
        this.nextNode = nextNode;
        this.match = match;
        this.game = game;
        this.qualifiers = qualifiers;
        this.olympics = olympics;
        this.nations = nations;
        this.texts = texts;
        this.flags = flags;
        this.round = round;
        this.players = players;
        this.soundtrack = soundtrack;
        this.worldCupGroup = worldCupGroup;
        this.worldCupDraw = worldCupDraw;
    }

    public void RunNode(List<Player> victors)
    {
        Console.WriteLine("Player1 : " + player1.name);
        Console.WriteLine("Player2 : " + player2.name);
        string context;
        if (olympics != null || qualifiers != null)
        {
            if (round <= 8)
            {
                player1.currentContext = players.contexts[round];
                player2.currentContext = players.contexts[round];
                //player1.currentContext = players.contexts[round];
            }
            
            context = players.contexts[round];
            if (context == "Silver") context = "Finals";
        }
        else
        {
            context = nations.worldCupContexts[round];
        }
        this.victors = victors;
        if (player1.NPC && player2.NPC)
        {
            SimNode();
        }
        else if (!player1.NPC && !player2.NPC)
        {
            if (soundtrack != null) soundtrack.StopSoundtrack();
            match.StartMatch(player1, player2, this, context);
        }
        else
        {
            if (soundtrack != null) soundtrack.StopSoundtrack();
            if (player1.NPC) game.StartGame(player2, player1, this, context);
            else game.StartGame(player1, player2, this, context);
        }
    }

    private void SimNode() 
    {
        int[] score = new int[4] { 0, 0, 0, 0 };
        int chances = 5;
        while (chances > 0)
        {
            int i = UnityEngine.Random.Range(1, 101);
            if (i <= player1.goalScorePercentage)
            {
                score[0]++;
                if (olympics != null) players.IncrementOlympicGoalScored(player1);
                else if (worldCupGroup != null)
                {
                    nations.GetNation(player1.nationality).worldCupGroupStagesGoals++;
                    player1.worldCupGoals++;
                }
                else if (worldCupDraw != null)
                {
                    player1.worldCupGoals++;
                }
            }
            else if (olympics != null) players.IncrementOlympicGoalMissed(player1);
                i = UnityEngine.Random.Range(1, 101);
            if (i <= player2.goalScorePercentage)
            {
                score[1]++;
                if (olympics != null) players.IncrementOlympicGoalScored(player2);
                else if (worldCupGroup != null)
                {
                    nations.GetNation(player2.nationality).worldCupGroupStagesGoals++;
                    player2.worldCupGoals++;
                }
                else if (worldCupDraw != null)
                {
                    player2.worldCupGoals++;
                }
            }
            else if (olympics != null) players.IncrementOlympicGoalMissed(player2);
                chances--;
        }
        if (score[0] != score[1]) ConcludeNode(score);
        else
        {
            int[] penaltiesScore = new int[2] { 0, 0 };
            int penaltyChances = 3;
            while (penaltyChances > 0) 
            {
                int i = UnityEngine.Random.Range(1, 101);
                if (i <= player1.penaltyScorePercentage)
                {
                    penaltiesScore[0]++;
                    if (olympics != null) players.IncrementOlympicPenaltyScored(player1);
                }
                else if (olympics != null) players.IncrementOlympicPenaltyMissed(player1);
                i = UnityEngine.Random.Range(1, 101);
                if (i <= player2.penaltyScorePercentage)
                {
                    penaltiesScore[1]++;
                    if (olympics != null) players.IncrementOlympicPenaltyScored(player2);
                }
                else if (olympics != null) players.IncrementOlympicPenaltyMissed(player2);
                penaltyChances--;
            }
            while (penaltiesScore[0] == penaltiesScore[1])
            {
                int i = UnityEngine.Random.Range(1, 101);
                if (i <= player1.penaltyScorePercentage)
                {
                    penaltiesScore[0]++;
                    if (olympics != null) players.IncrementOlympicPenaltyScored(player1);
                }
                else if (olympics != null) players.IncrementOlympicPenaltyMissed(player1);
                i = UnityEngine.Random.Range(1, 101);
                if (i <= player2.penaltyScorePercentage)
                {
                    penaltiesScore[1]++;
                    if (olympics != null) players.IncrementOlympicPenaltyScored(player2);
                }
                else if (olympics != null) players.IncrementOlympicPenaltyMissed(player2);
            }
            //if (penaltiesScore[0] > penaltiesScore[1]) score[0]++;
            //else score[1]++;
            score[2] = penaltiesScore[0];
            score[3] = penaltiesScore[1];
            if (olympics != null)
            {
                if (score[2] > score[3])
                {
                    players.IncrementOlympicPenaltyShootOutWin(player1);
                    players.IncrementOlympicPenaltyShootOutLose(player2);
                }
                else
                {
                    players.IncrementOlympicPenaltyShootOutWin(player2);
                    players.IncrementOlympicPenaltyShootOutLose(player1);
                }
            }
            ConcludeNode(score);
        }
    }

    public void ConcludeNode(int[] score)
    {
        Player winner;
        Player loser;
        if (player1.NPC && !player2.NPC) 
        {
            int x = score[1];
            score[1] = score[0];
            score[0] = x;
            int y = score[3];
            score[3] = score[2];
            score[2] = y;
        }
        if (score[0] > score[1] || score[2] > score[3])
        {
            winner = player1;
            loser = player2;
        }
        else
        {
            winner = player2;
            loser = player1;
        }
        if (olympics != null)
        {
            players.IncrementOlympicWin(winner);
            players.IncrementOlympicLose(loser);
        }
        if (texts != null)
        {
            if (texts.Length >= 3)
            {
                texts[2].text = winner.name;
                if (score[2] != 0 || score[3] != 0)
                {
                    texts[0].text = Mathf.Min(score[0], score[1]) + "(" + score[2] + ")";
                    texts[1].text = Mathf.Min(score[0], score[1]) + "(" + score[3] + ")";
                }
                else
                {
                    texts[0].text = score[0].ToString();
                    texts[1].text = score[1].ToString();
                }
                if (texts.Length == 4)
                {
                    texts[3].text = loser.name;
                    flags[1].texture = nations.GetNation(loser.nationality).flag;
                }
            }
            else
            {
                texts[0].text = winner.name;
            }
            //if (texts.Length == 4)
            //{
            //    texts[0].text = score[0].ToString();
            //    texts[1].text = score[1].ToString();
            //    texts[2].text = winner.name;
            //    texts[3].text = loser.name;
            //    flags[1].texture = nations.GetNation(loser.nationality).flag;
            //}
            //else if (texts.Length == 3) //you mean 5 bruh and what about flags
            //{
            //    texts[0].text = score[0].ToString();
            //    texts[1].text = score[1].ToString();
            //    texts[2].text = winner.name;               
            //}
            //else
            //{
            //    
            //}
            flags[0].texture = nations.GetNation(winner.nationality).flag;
        }
        //Console.WriteLine(winner.name + " defeated " + loser.name + " " + Mathf.Max(score[0], score[1]) + ":" + Mathf.Min(score[0], score[1]));
        victors.Add(winner);
        if (nextNode != null)
        {
            nextNode.RunNode(victors);
        }
        else
        {
            if (qualifiers != null)
            {
                qualifiers.AnalyzeNodes(victors);
            }
            else if (olympics != null)
            {
                olympics.AnalyzeNodes(victors.ToArray());
            }
            else if (worldCupGroup != null)
            {
                worldCupGroup.AnalyzeNodes(victors.ToArray());
            }
            else
            {
                worldCupDraw.AnalyzeNodes(victors.ToArray());
            }
        }
    }
}
