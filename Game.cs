using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Game : MonoBehaviour
{
    [SerializeField] Nations nations;
    [SerializeField] Players players;
    [SerializeField] Text playerText;
    [SerializeField] Text NPCText;
    [SerializeField] Text scoreText;
    [SerializeField] Text penaltiesText;
    [SerializeField] Button penaltiesButton;
    [SerializeField] Button victoryButton;
    [SerializeField] RawImage[] playerBalls;
    [SerializeField] RawImage[] NPCBalls;
    [SerializeField] RawImage NPCFlag;
    [SerializeField] Player player;
    [SerializeField] Player NPC;
    [SerializeField] Node node;
    [SerializeField] Text context;
    [SerializeField] Button hit;
    [SerializeField] Button miss;
    [SerializeField] Text playerContext;
    [SerializeField] Text NPCContext;
    [SerializeField] RawImage playerMedal;
    [SerializeField] RawImage NPCMedal;
    [SerializeField] Texture goldMedal;
    [SerializeField] Texture silverMedal;
    [SerializeField] Texture bronzeMedal;
    [SerializeField] RawImage rings;

    int[] score;
    int[] penaltiesScore;
    int numShot = 0;
    bool suddenDeath = false;
    int[] playerShots;
    int[] NPCShots;
    bool olympics;
    bool worldCupGroups;
    bool worldCupDraw;
    public void StartGame(Player player, Player NPC, Node node, string context)
    {
        GetComponent<Canvas>().enabled = true;
        playerShots = new int[1000];
        NPCShots = new int[1000];
        for (int i = 0; i < playerShots.Length; i++)
        {
            playerShots[i] = 0;
            NPCShots[i] = 0;
        }
        this.context.text = context;
        this.player = player;
        this.NPC = NPC;
        this.node = node;
        if (player.olympicSeed == 0) playerText.text = player.name;
        else playerText.text = "#" + player.olympicSeed + " " + player.name;
        if (NPC.olympicSeed == 0) NPCText.text = NPC.name;
        else NPCText.text = "#" + NPC.olympicSeed + " " + NPC.name;
        scoreText.text = "0:0";
        score = new int[4] { 0, 0, 0, 0 };
        penaltiesScore = new int[2] { 0, 0 };
        numShot = 0;
        Texture flag = nations.GetNation(NPC.nationality).flag;
        if (flag != null) NPCFlag.GetComponent<RawImage>().texture = flag;
        for (int i = 0; i < 5; i++)
        {
            playerBalls[i].color = Color.white;
            playerBalls[i].GetComponent<RawImage>().enabled = true;
            NPCBalls[i].color = Color.white;
            NPCBalls[i].GetComponent<RawImage>().enabled = true;
        }
        suddenDeath = false;
        hit.gameObject.SetActive(true);
        miss.gameObject.SetActive(true);
        playerContext.text = "Best result: " + player.context;
        NPCContext.text = "Best result: " + NPC.context;
        if (player.context == "Gold") playerMedal.GetComponent<RawImage>().texture = goldMedal;
        else if (player.context == "Silver") playerMedal.GetComponent<RawImage>().texture = silverMedal;
        else if (player.context == "Bronze") playerMedal.GetComponent<RawImage>().texture = bronzeMedal;
        else playerMedal.GetComponent<RawImage>().texture = null;
        if (NPC.context == "Gold") NPCMedal.GetComponent<RawImage>().texture = goldMedal;
        else if (NPC.context == "Silver") NPCMedal.GetComponent<RawImage>().texture = silverMedal;
        else if (NPC.context == "Bronze") NPCMedal.GetComponent<RawImage>().texture = bronzeMedal;
        else NPCMedal.GetComponent<RawImage>().texture = null;
        if (node.olympics != null) rings.GetComponent<RawImage>().enabled = true;
        else rings.GetComponent<RawImage>().enabled = false;
        olympics = node.olympics != null;
        worldCupGroups = node.worldCupGroup != null;
        worldCupDraw = node.worldCupDraw != null;

    }

    public void Shoot(bool scored) //disable on activation of confirm button
    {
        numShot++;
        if (numShot < 6)
        {
            if (scored)
            {
                playerShots[numShot] = 2;
                playerBalls[numShot - 1].color = Color.green;
                if (olympics) players.IncrementOlympicGoalScored(player);
                else if (worldCupGroups)
                {
                    nations.GetNation(player.nationality).worldCupGroupStagesGoals++;
                    player.worldCupGoals++;
                }
                else if (worldCupDraw)
                {
                    player.worldCupGoals++;
                }
                score[0]++;
            }
            else 
            {
                playerShots[numShot] = 1;
                playerBalls[numShot - 1].color = Color.red;
                if (olympics) players.IncrementOlympicGoalMissed(player);
            }
            if (NPCShots[numShot] == 0)
            {
                int i = UnityEngine.Random.Range(1, 101);
                if (i <= NPC.goalScorePercentage)
                {
                    NPCScore();
                }
                else
                {
                    NPCMiss();
                }
            }
            else if (NPCShots[numShot] == 1) NPCMiss();
            else NPCScore();
            if (numShot == 5)
            {
                hit.gameObject.SetActive(false);
                miss.gameObject.SetActive(false);
                if (score[0] != score[1])
                {
                    ConcludeGame();                   
                }
                else
                {
                    Conclude5Shots();
                }
            }
            else penaltiesButton.gameObject.SetActive(false);
        }
        else
        {
            if (scored)
            {
                playerShots[numShot] = 2;
                penaltiesScore[0]++;
                if (olympics) players.IncrementOlympicPenaltyScored(player);
                if (!suddenDeath)
                {
                    playerBalls[numShot - 6].color = Color.green;
                }
                else
                {
                    playerBalls[0].color = Color.green;
                }
            }
            else
            {
                playerShots[numShot] = 1;
                if (suddenDeath)
                {
                    playerBalls[0].color = Color.red;
                }
                else
                {
                    playerBalls[numShot - 6].color = Color.red;
                }
                if (olympics) players.IncrementOlympicPenaltyMissed(player);
            }
            if (NPCShots[numShot] == 0)
            {
                int i = UnityEngine.Random.Range(0, 101);
                if (i <= NPC.penaltyScorePercentage)
                {
                    NPCPenaltyScore();
                }
                else
                {
                    NPCPenaltyMiss();
                }
            }
            else if (NPCShots[numShot] == 1) NPCPenaltyMiss();
            else NPCPenaltyScore(); 
                if (numShot > 7)
            {
                //numShot = 7;
                if (penaltiesScore[0] != penaltiesScore[1])
                {
                    if (olympics)
                    {
                        if (penaltiesScore[0] > penaltiesScore[1])
                        {
                            players.IncrementOlympicPenaltyShootOutWin(player);
                            players.IncrementOlympicPenaltyShootOutLose(NPC);
                        }
                        else
                        {
                            players.IncrementOlympicPenaltyShootOutWin(NPC);
                            players.IncrementOlympicPenaltyShootOutLose(player);
                        }
                    }
                  ConcludeGame();
                }
                if (numShot > 8)
                {
                    for (int j = 1; j < 3; j++)
                    {
                        playerBalls[j].GetComponent<RawImage>().enabled = false;
                        NPCBalls[j].GetComponent<RawImage>().enabled = false;
                    }
                }               
                suddenDeath = true;
            }
            penaltiesText.text = penaltiesScore[0] + ":" + penaltiesScore[1];
        }
        scoreText.text = score[0] + ":" + score[1];
    }

    private void Conclude5Shots()
    {
        victoryButton.gameObject.SetActive(false);
        penaltiesButton.gameObject.SetActive(true);
        hit.gameObject.SetActive(false);
        miss.gameObject.SetActive(false);
    }

    private void NPCPenaltyMiss()
    {
        NPCShots[numShot] = 1;
        if (olympics) players.IncrementOlympicPenaltyMissed(NPC);
        if (suddenDeath) NPCBalls[0].color = Color.red;
        else NPCBalls[numShot - 6].color = Color.red;
    }

    private void NPCPenaltyScore()
    {
        NPCShots[numShot] = 2;
        penaltiesScore[1]++;
        if (olympics) players.IncrementOlympicPenaltyScored(NPC);
        if (suddenDeath) NPCBalls[0].color = Color.green;
        else NPCBalls[numShot - 6].color = Color.green;
    }

    private void NPCMiss()
    {
        NPCShots[numShot] = 1;
        NPCBalls[numShot - 1].color = Color.red;
        if (olympics) players.IncrementOlympicGoalMissed(NPC);
    }

    private void NPCScore()
    {
        NPCShots[numShot] = 2;
        score[1]++;
        if (olympics) players.IncrementOlympicGoalScored(NPC);
        else if (worldCupGroups)
        {
            nations.GetNation(NPC.nationality).worldCupGroupStagesGoals++;
            NPC.worldCupGoals++;
        }
        else if (worldCupDraw)
        {
            NPC.worldCupGoals++;
        }
        NPCBalls[numShot - 1].color = Color.green;
    }

    public void StartPenalties()
    {
        penaltiesButton.gameObject.SetActive(false);
        hit.gameObject.SetActive(true);
        miss.gameObject.SetActive(true);
        penaltiesText.GetComponent<Text>().enabled = true;
        penaltiesText.text = "penalties: 0:0";
        for (int i = 0; i < 5; i++)
        {
            if (i >= 3)
            {
                playerBalls[i].GetComponent<RawImage>().enabled = false;
                NPCBalls[i].GetComponent<RawImage>().enabled = false;
            }
            else
            {
                playerBalls[i].GetComponent<RawImage>().color = Color.white;
                NPCBalls[i].GetComponent<RawImage>().color = Color.white;
            }
        }
        penaltiesScore = new int[2] { 0, 0 };
    }

    private void ConcludeGame()
    {       
        victoryButton.gameObject.SetActive(true);
        hit.gameObject.SetActive(false);
        miss.gameObject.SetActive(false);
        score[2] = penaltiesScore[0];
        score[3] = penaltiesScore[1];
        if (score[0] > score[1] || score[2] > score[3])
        {
            if (score[0] > score[1]) victoryButton.GetComponentInChildren<Text>().text = "Confirm: " + player.name + " defeated " + NPC.name + " " + score[0] + ":" + score[1];
            else victoryButton.GetComponentInChildren<Text>().text = "Confirm: " + player.name + " defeated " + NPC.name + " " + score[0] + ":" + score[1] + " by penalties (" + score[2] + ":" + score[3] + ")";
        }
        else 
        {
            if (score[1] > score[0]) victoryButton.GetComponentInChildren<Text>().text = "Confirm: " + NPC.name + " defeated " + player.name + " " + score[1] + ":" + score[0];
            else victoryButton.GetComponentInChildren<Text>().text = "Confirm: " + NPC.name + " defeated " + player.name + " " + score[1] + ":" + score[0] + " by penalties (" + score[3] + ":" + score[2] + ")";
        }
    }

    public void Cancel()
    {
        victoryButton.gameObject.SetActive(false);
        penaltiesButton.gameObject.SetActive(false);
        hit.gameObject.SetActive(true);
        miss.gameObject.SetActive(true);
        if (numShot < 6)
        {
            if (playerShots[numShot] == 2)
            {
                score[0]--;
                if (olympics) player.OlympicGoalScored--;
            }
            else if (olympics) player.OlympicGoalMissed--;
            players.UpdatePlayerGoalScorePercentage(player);
            if (NPCShots[numShot] == 2)
            {
                score[1]--;
                if (olympics) NPC.OlympicGoalScored--;
            }
            else if (olympics) NPC.OlympicGoalMissed--;
            players.UpdatePlayerGoalScorePercentage(player);
            playerBalls[numShot - 1].color = Color.white;
            NPCBalls[numShot - 1].color = Color.white;
        }
        else
        {
            if (playerShots[numShot] == 2)
            {
                penaltiesScore[0]--;
                if (olympics) player.OlympicPenaltyScored--;
            }
            else if (olympics) player.OlympicPenaltyMissed--;
            players.UpdatePlayerPenaltyScorePercentage(player);
            if (NPCShots[numShot] == 2)
            {
                penaltiesScore[1]--;
                if (olympics) NPC.OlympicPenaltyScored--;
            }
            else if (olympics) NPC.OlympicPenaltyMissed--;
            players.UpdatePlayerPenaltyScorePercentage(NPC);
            if (numShot < 8)
            {
                playerBalls[numShot - 6].color = Color.white;
                NPCBalls[numShot - 6].color = Color.white;
            }
            else
            {
                if (numShot == 8)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        playerBalls[i].GetComponent<RawImage>().enabled = true;
                        NPCBalls[i].GetComponent<RawImage>().enabled = true;
                        if (i == 2)
                        {
                            playerBalls[i].color = Color.white;
                            NPCBalls[i].color = Color.white;
                        }
                        else 
                        {
                            if (playerShots[6 + i] == 1) playerBalls[i].color = Color.red;
                            else playerBalls[i].color = Color.green;
                            if (NPCShots[6 + i] == 1) NPCBalls[i].color = Color.red;
                            else NPCBalls[i].color = Color.green;
                        }
                    }
                }
                else
                {
                    playerBalls[0].color = Color.white;
                    NPCBalls[0].color = Color.white;
                }
            }
        }
        numShot--;
        scoreText.text = score[0] + ":" + score[1];
        penaltiesText.text = penaltiesScore[0] + ":" + penaltiesScore[1];
    }

    public void ConfirmGame()
    {
        penaltiesText.GetComponent<Text>().enabled = false;
        GetComponent<Canvas>().enabled = false;
        victoryButton.gameObject.SetActive(false);
        if (node != null) node.ConcludeNode(score);
    }
}
