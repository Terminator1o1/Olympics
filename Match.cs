using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Match : MonoBehaviour
{
    [SerializeField] Text player1Text;
    [SerializeField] Text player2Text;
    [SerializeField] Text scoreBoard;
    [SerializeField] Player firstPlayer;
    [SerializeField] Player secondPlayer;
    [SerializeField] int[] score;
    [SerializeField] int[] penaltiesScore;
    [SerializeField] Players players;
    [SerializeField] Button victoryButton;
    [SerializeField] GameObject penaltiesButton;
    [SerializeField] Text penaltiesText;
    [SerializeField] Text penaltyShooter;
    [SerializeField] GameObject penalties;
    [SerializeField] bool player1Turn;
    [SerializeField] int player1PenaltyNum = 0;
    [SerializeField] int player2PenaltyNum = 0;
    [SerializeField] Button penaltiesVictory;
    [SerializeField] Championship championship;
    [SerializeField] Node node;
    [SerializeField] Text context;
    [SerializeField] Button[] olympicButtons;
    [SerializeField] Text player1Context;
    [SerializeField] Text player2Context;
    [SerializeField] RawImage player1Medal;
    [SerializeField] RawImage player2Medal;
    [SerializeField] Texture goldMedal;
    [SerializeField] Texture silverMedal;
    [SerializeField] Texture bronzeMedal;
    [SerializeField] RawImage rings;
    [SerializeField] Button player1Goal;
    [SerializeField] Button player2Goal;
    [SerializeField] Button shootOutScoreButton;
    [SerializeField] Button shootOutMissButton;
    [SerializeField] Button player1Decrement;
    [SerializeField] Button player2Decrement;
    bool olympics;
    public void StartMatch(Player player1, Player player2, Node node, string context)
    {
        firstPlayer = player1;
        secondPlayer = player2;
        this.node = node;
        SetText(player1Text, firstPlayer);
        SetText(player2Text, secondPlayer);
        scoreBoard.text = "0:0";
        score = new int[4] { 0, 0, 0 , 0 };
        penaltiesScore = new int[2] { 0, 0 };
        this.context.text = context;
        if (node != null && context != "Qualifiers First Round" && context != "Qualifiers Second Round") olympics = true;
        else olympics = false;
        ToggleAllButtons(olympics);
        player1Goal.gameObject.SetActive(true);
        player2Goal.gameObject.SetActive(true);
        player1Decrement.gameObject.SetActive(true);
        player2Decrement.gameObject.SetActive(true);
        rings.GetComponent<RawImage>().enabled = olympics;
        player1Context.text = "Best result: " + player1.context;
        //else if (player1.championshipFinals != 0) player1Context.text = "Best result: Championship Final";
        //else player1Context.text = "Best result: Championship Participant";  
        player2Context.text = "Best result: " + player2.context;
        //else if (player2.championshipFinals != 0) player2Context.text = "Best result: Championship Final";
        //else player2Context.text = "Best result: Championship Participant";
        if (player1.context == "Gold") player1Medal.GetComponent<RawImage>().texture = goldMedal;
        else if (player1.context == "Silver") player1Medal.GetComponent<RawImage>().texture = silverMedal;
        else if (player1.context == "Bronze") player1Medal.GetComponent<RawImage>().texture = bronzeMedal;
        else player1Medal.GetComponent<RawImage>().texture = null;
        if (player2.context == "Gold") player2Medal.GetComponent<RawImage>().texture = goldMedal;
        else if (player2.context == "Silver") player2Medal.GetComponent<RawImage>().texture = silverMedal;
        else if (player2.context == "Bronze") player2Medal.GetComponent<RawImage>().texture = bronzeMedal;
        else player2Medal.GetComponent<RawImage>().texture = null;
        GetComponent<Canvas>().enabled = true;
    }

    private void SetText(Text playerText, Player player)
    {
        if (player.olympicSeed == 0) playerText.text = player.name;
        else playerText.text = "#" + player.olympicSeed + " " + player.name;
    }

    public void IncrementPlayer1()
    {
        score[0]++;
        //if (olympics) players.IncrementOlympicGoalScored(firstPlayer);
        UpdateScoreSheet();
    }

    public void IncrementPlayer2()
    {
        score[1]++;
        //if (olympics) players.IncrementOlympicGoalScored(secondPlayer);
        UpdateScoreSheet();
    }

    public void DoubleIncrement()
    {
        //players.IncrementChampionshipMatchGoals(firstPlayer);
        //players.IncrementChampionshipMatchGoals(secondPlayer);
    }

    public void PenaltyIncrementPlayer1()
    {
        score[0]++;
        players.IncrementOlympicPenaltyScored(firstPlayer);
        UpdateScoreSheet();
    }

    public void PenaltyIncrementPlayer2()
    {
        score[1]++;
        players.IncrementOlympicPenaltyScored(secondPlayer);
        UpdateScoreSheet();
    }

    public void PenaltyMissedPlayer1()
    {
        players.IncrementOlympicPenaltyMissed(firstPlayer);
    }

    public void PenaltyMissedPlayer2()
    {
        players.IncrementOlympicPenaltyMissed(secondPlayer);
    }
    private void UpdateScoreSheet()
    {
        scoreBoard.text = score[0] + ":" + score[1];
        if (MatchOver())
        {
            if (score[0] > score[1] || score[2] > score[3])
            {
                if (score[2] > score[3]) victoryButton.GetComponentInChildren<Text>().text = "Confirm: " + firstPlayer.name + " defeated " + secondPlayer.name + " " + score[0] + ":" + score[1] + " by penalties (" + score[2] + ":" + score[3] + ")";
                else victoryButton.GetComponentInChildren<Text>().text = "Confirm: " + firstPlayer.name + " defeated " + secondPlayer.name + " " + score[0] + ":" + score[1];
            }
            else
            {
                if (score[3] > score[2]) victoryButton.GetComponentInChildren<Text>().text = "Confirm: " + secondPlayer.name + " defeated " + firstPlayer.name + " " + score[1] + ":" + score[0] + " by penalties (" + score[3] + ":" + score[2] + ")";
                else victoryButton.GetComponentInChildren<Text>().text = "Confirm: " + secondPlayer.name + " defeated " + firstPlayer.name + " " + score[1] + ":" + score[0];
            }
            victoryButton.gameObject.SetActive(true);
            ToggleAllButtons(false);
            if (score[2] == score[3])
            {
                player1Decrement.gameObject.SetActive(true);
                player2Decrement.gameObject.SetActive(true);
                if (olympics)
                {
                    for (int i = 4; i < 8; i++)
                    {
                        olympicButtons[i].gameObject.SetActive(true);
                    }
                }              
            }
        }
        else
        {
            victoryButton.gameObject.SetActive(false);
            if (score[0] == score[1] && score[0] == 3)
            {
                penaltiesButton.gameObject.SetActive(true);
                player1Goal.gameObject.SetActive(false);
                player2Goal.gameObject.SetActive(false);
                olympicButtons[0].gameObject.SetActive(false);
                olympicButtons[1].gameObject.SetActive(false);
                olympicButtons[2].gameObject.SetActive(false);
                olympicButtons[3].gameObject.SetActive(false);
            }
            else
            {
                penaltiesButton.gameObject.SetActive(false);
                ToggleAllButtons(olympics);
                player1Goal.gameObject.SetActive(true);
                player2Goal.gameObject.SetActive(true);
                player1Decrement.gameObject.SetActive(true);
                player2Decrement.gameObject.SetActive(true);
            }
        }
    }

    private void ToggleAllButtons(bool v)
    {
        player1Goal.gameObject.SetActive(v);
        player2Goal.gameObject.SetActive(v);
        player1Decrement.gameObject.SetActive(v);
        player2Decrement.gameObject.SetActive(v);
        foreach (Button button in olympicButtons)
        {
            button.gameObject.SetActive(v);
        }
    }

    public void BeginPenalties()
    {
        penaltiesButton.SetActive(false);
        ToggleAllButtons(false);
        penaltiesText.text = "penalties: 0:0";
        penaltyShooter.text = firstPlayer.name;
        penalties.SetActive(true);
        shootOutMissButton.gameObject.SetActive(true);
        shootOutScoreButton.gameObject.SetActive(true);
        player1Turn = true;
        player1PenaltyNum = 0;
        player2PenaltyNum = 0;
    }

    public void PenaltyScored(bool scored)
    {
        player1Turn = !player1Turn;
        if (player1Turn) penaltyShooter.text = firstPlayer.name;
        else penaltyShooter.text = secondPlayer.name;       

        if (!player1Turn) player1PenaltyNum++; //switched
        else player2PenaltyNum++;

        if (scored)
        {
            if (!player1Turn) //switched
            {
                penaltiesScore[0]++;
                if (node != null && context.text != "Qualifiers First Round" && context.text != "Qualifiers Second Round") players.IncrementOlympicPenaltyScored(firstPlayer);
            }
            else
            {
                penaltiesScore[1]++;
                if (node != null && context.text != "Qualifiers First Round" && context.text != "Qualifiers Second Round") players.IncrementOlympicPenaltyScored(secondPlayer);
            }
        }
        else //switched here too
        {
            if (!player1Turn && node != null && context.text != "Qualifiers First Round" && context.text != "Qualifiers Second Round") players.IncrementOlympicPenaltyMissed(firstPlayer);
            else if (player1Turn && node != null && context.text != "Qualifiers First Round" && context.text != "Qualifiers Second Round") players.IncrementOlympicPenaltyMissed(secondPlayer);
        }
        penaltiesText.text = "penalties: " + penaltiesScore[0] + ":" + penaltiesScore[1];
        if (player2PenaltyNum > 2 && player1PenaltyNum == player2PenaltyNum && penaltiesScore[0] - penaltiesScore[1] != 0)
        {
            ConcludePenalties();
        }
        else if (player2PenaltyNum < 3 && (penaltiesScore[0] > penaltiesScore[1] + 3 - player2PenaltyNum || penaltiesScore[1] > penaltiesScore[0] + 3 - player1PenaltyNum))
        {
            ConcludePenalties();
        }
        else
        {
            penaltiesVictory.gameObject.SetActive(false);                          
        }
    }

    private void ConcludePenalties()
    {
        penaltiesText.text = "penalties: " + penaltiesScore[0] + ":" + penaltiesScore[1];
        if (penaltiesScore[0] > penaltiesScore[1])
        {
            penaltiesVictory.GetComponentInChildren<Text>().text = "Confirm: " + firstPlayer.name + " defeated " + secondPlayer.name + " on penalties " + penaltiesScore[0] + ":" + penaltiesScore[1];
        }
        else
        {
            penaltiesVictory.GetComponentInChildren<Text>().text = "Confirm: " + secondPlayer.name + " defeated " + firstPlayer.name + " on penalties " + penaltiesScore[1] + ":" + penaltiesScore[0];
        }
        penaltiesVictory.gameObject.SetActive(true);
        shootOutScoreButton.gameObject.SetActive(false);
        shootOutMissButton.gameObject.SetActive(false);
    }

    public void ConfirmPenalties()
    {
        penalties.SetActive(false);
        penaltiesVictory.gameObject.SetActive(false);
        score[2] = penaltiesScore[0];
        score[3] = penaltiesScore[1];
        if (penaltiesScore[0] > penaltiesScore[1])
        {
            //score[0]++;
            if (node != null && context.text != "Qualifiers First Round" && context.text != "Qualifiers Second Round")
            {
                players.IncrementOlympicPenaltyShootOutWin(firstPlayer);
                players.IncrementOlympicPenaltyShootOutLose(secondPlayer);
            }
            UpdateScoreSheet();
        }
        else
        {
            //score[1]++;
            if (node != null && context.text != "Qualifiers First Round" && context.text != "Qualifiers Second Round")
            {
                players.IncrementOlympicPenaltyShootOutWin(secondPlayer);
                players.IncrementOlympicPenaltyShootOutLose(firstPlayer);
            }
            UpdateScoreSheet();
        }
    }

    public void ConfirmMatch()
    {
        victoryButton.gameObject.SetActive(false);
        GetComponent<Canvas>().enabled = false;
        if (node == null)
        {
            championship.AnalyzeMatch(score);
        }
        else
        {
            node.ConcludeNode(score);
        }
    }

    public void DecrementPlayer1()
    {    
        score[0]--;
        //if (olympics)
        //{
        //    firstPlayer.OlympicGoalScored--;
        //    players.UpdatePlayerGoalScorePercentage(firstPlayer);
        //}
        UpdateScoreSheet();
        penaltiesButton.gameObject.SetActive(false);
        player1Goal.gameObject.SetActive(true);
        player2Goal.gameObject.SetActive(true);
    }

    public void DecrementPlayer2()
    {
        score[1]--;
        //if (olympics)
        //{
        //    secondPlayer.OlympicGoalScored--;
        //    players.UpdatePlayerGoalScorePercentage(secondPlayer);
        //}
        UpdateScoreSheet();
        penaltiesButton.gameObject.SetActive(false);
        player1Goal.gameObject.SetActive(true);
        player2Goal.gameObject.SetActive(true);
    }

    public void PenaltyDecrementPlayer1() //must be olympics
    {
        firstPlayer.OlympicPenaltyScored--;
        players.UpdatePlayerPenaltyScorePercentage(firstPlayer);
        score[0]--;
        UpdateScoreSheet();
    }

    public void PenaltyDecrementPlayer2() // ^^^
    {
        secondPlayer.OlympicPenaltyScored--;
        players.UpdatePlayerPenaltyScorePercentage(secondPlayer);
        score[1]--;
        UpdateScoreSheet();
    }

    public void CancelPenaltyMissedPlayer1()
    {
        firstPlayer.OlympicPenaltyMissed--;
        players.UpdatePlayerPenaltyScorePercentage(firstPlayer);
    }

    public void CancelPenaltyMissedPlayer2()
    {
        secondPlayer.OlympicPenaltyMissed--;
        players.UpdatePlayerPenaltyScorePercentage(secondPlayer);
    }

    public void CancelShootOutScore()
    {
        if (player1Turn)
        {
            penaltiesScore[1]--;
            player2PenaltyNum--;
            penaltyShooter.text = secondPlayer.name;
        }
        else
        {
            penaltiesScore[0]--;
            player1PenaltyNum--;
            penaltyShooter.text = firstPlayer.name;
        }
        player1Turn = !player1Turn;
        penaltiesVictory.gameObject.SetActive(false);
        penalties.gameObject.SetActive(true);
        shootOutScoreButton.gameObject.SetActive(true);
        shootOutMissButton.gameObject.SetActive(true);
        penaltiesText.text = "penalties: " + penaltiesScore[0] + ":" + penaltiesScore[1];
    }

    public void CancelShootOutMiss()
    {
        if (player1Turn)
        {
            player2PenaltyNum--;
            penaltyShooter.text = secondPlayer.name;
        }
        else
        {
            player1PenaltyNum--;
            penaltyShooter.text = firstPlayer.name;
        }
        player1Turn = !player1Turn;
        penaltiesVictory.gameObject.SetActive(false);
        penalties.gameObject.SetActive(true);
        shootOutScoreButton.gameObject.SetActive(true);
        shootOutMissButton.gameObject.SetActive(true);
    }
    private bool MatchOver()
    {
        if (score[0] > 3 || score[1] > 3) return true;
        if ((score[0] == 3 || score[1] == 3) && Mathf.Abs(score[0] - score[1]) > 1) return true;
        if (score[0] == 3 && score[1] == 3 && score[2] != score[3]) return true;
        return false;
    }
}