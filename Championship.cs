using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Championship : MonoBehaviour
{
    [SerializeField] Players players;
    [SerializeField] Player first;
    [SerializeField] Player second;
    [SerializeField] Player third;
    [SerializeField] int matchNum = 1;
    [SerializeField] Match match;
    [SerializeField] int[] matching;
    [SerializeField] int[] victories;
    [SerializeField] int[] goalDifference;
    [SerializeField] int[] goals;
    [SerializeField] Player houseWinner;
    [SerializeField] Player houseLoser;
    [SerializeField] Player houseRunnerUp;
    [SerializeField] int[] matchWinner;
    [SerializeField] int[] matchLoser;
    [SerializeField] Canvas championshipCanvas;
    [SerializeField] Text[] playerNames;
    [SerializeField] Text[] playerVictories;
    [SerializeField] Text[] playerGoalDifference;
    [SerializeField] Text[] playerGoals;
    [SerializeField] Canvas winCanvas;
    [SerializeField] int[] potentialGoals;
    [SerializeField] int[] goalPercentage;
    [SerializeField] Soundtrack soundtrack;
    //[SerializeField] VideoPlayer video;
    //[SerializeField] Canvas videoCanvas;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartChampionship()
    {
        InitializePlayers();
        //print("First: " + first.name + " Second: " + second.name + " Third: " + third.name);
        //Player[] secondMatch = { third, first };
        //Player[] thirdMatch = { second, first };
        matching = new int[2] { 3, 2 };
        victories = new int[4] { 0, 0, 0 , 0};
        goalDifference = new int[4] { 0, 0, 0, 0 };
        goals = new int[4] { 0, 0, 0 , 0};
        potentialGoals = new int[3] { 0, 0, 0 };
        goalPercentage = new int[3] { 0, 0, 0 };
        matchWinner = new int[4];
        matchLoser = new int[4];
        matchNum = 1;
        //int[] result = new int[2] { 0, 0 };
        //results = new int[][] { result, result, result};
        match.StartMatch(third, second, null, "Locals");
    }
    public void AnalyzeMatch(int[] score)
    {
        if (score[0] < score[1] || score[2] < score[3])
        {
            matchWinner[matchNum - 1] = matching[1];
            matchLoser[matchNum - 1] = matching[0];
            victories[matching[1] - 1]++;
        }
        else
        {
            matchWinner[matchNum - 1] = matching[0];
            matchLoser[matchNum - 1] = matching[1];
            victories[matching[0] - 1]++;
        }
        goalDifference[matching[0] - 1] += score[0] - score[1];
        goalDifference[matching[1] - 1] += score[1] - score[0];
        goals[matching[0] - 1] += score[0];
        goals[matching[1] - 1] += score[1];
        int possibleGoals = 3;
        if (score[0] == 4 || score[1] == 4) possibleGoals++;
        potentialGoals[matching[0] - 1] += possibleGoals;
        potentialGoals[matching[1] - 1] += possibleGoals;
        matchNum++;
        if (matchNum == 4)
        {
            for (int i = 0; i < 3; i++)
            {
                goalPercentage[i] =(int) (((double)goals[i] / potentialGoals[i]) * 100);
            }
            Player[] playerArray = new Player[3];
            int[] arrayRank = AnalyzeGroup();
            if (arrayRank[0] == -1)
            {
                playerArray[0] = first;
                playerArray[1] = second;
                playerArray[2] = third;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    if (arrayRank[i] == 1)
                    {
                        playerArray[i] = first;
                    }
                    else if (arrayRank[i] == 2)
                    {
                        playerArray[i] = second;
                    }
                    else
                    {
                        playerArray[i] = third;
                    }
                }
            }
            houseWinner = playerArray[0];
            houseRunnerUp = playerArray[1];
            houseLoser = playerArray[2];
            BuildTable(playerArray);
        }
        else if (matchNum == 3)
        {
            matching = new int[2] { 2, 1 };
            match.StartMatch(second, first, null, "Locals");
        }
        else if (matchNum == 2)
        {
            matching = new int[2] { 3, 1 };
            match.StartMatch(third, first, null, "Locals");
        }
        else
        {
            third = houseLoser;
            if (score[0] > score[1] || score[2] > score[3])
            {
                first = houseRunnerUp;
                second = houseWinner;
            }
            else
            {
                first = houseWinner;
                second = houseRunnerUp;
            }
            players.IncrementChampionshipWins(first);
            first.rank = "First";
            second.rank = "Second";
            third.rank = "Third";
            winCanvas.GetComponent<Canvas>().enabled = true;
            soundtrack.PlaySoundtrack();
        }
    }
    
    public void ConcludeChampionship()
    {
        winCanvas.GetComponent<Canvas>().enabled = false;
        soundtrack.StopSoundtrack();
        GetComponent<Locals>().RunLocals(first, second, third);   
    }

    private void BuildTable(Player[] arr)
    {
        for (int i = 0; i < 3; i++)
        {
            int j;
            if (arr[i] == first) j = 0;
            else if (arr[i] == second) j = 1;
            else j = 2;
            playerNames[i].text = arr[i].name;
            playerVictories[i].text = victories[j].ToString();
            playerGoalDifference[i].text = goalDifference[j].ToString();
            playerGoals[i].text = goals[j].ToString(); //?
        }
        championshipCanvas.GetComponent<Canvas>().enabled = true;
        soundtrack.PlaySoundtrack();
    }

    public void ConcludeGroup()
    {
        championshipCanvas.GetComponent<Canvas>().enabled = false;
        //soundtrack.StopSoundtrack();
        players.IncrementChampionshipFinals(houseRunnerUp);
        players.IncrementChampionshipFinals(houseWinner);
        players.IncrementFirstPlaceFinishers(houseWinner);
        match.StartMatch(houseRunnerUp, houseWinner, null, "Group Final");
    }

    private int[] AnalyzeGroup()
    {
        int[] array;
        if (victories[0] != victories[1] || victories[1] != victories[2]) array = victories;
        else if (goalDifference[0] != goalDifference[1] || goalDifference[1] != goalDifference[2]) array = goalDifference;
        //else if (goalPercentage[0] != goalPercentage[1] || goalPercentage[1] != goalPercentage[2]) array = goalPercentage;
        else
        {
            return new int[3] { -1, -1, -1 };
        }
        if (array[0] != array[1] && array[1] != array[2] && array[2] != array[0])
        {
            if (array[0] > array[1])
            {
                if (array[1] > array[2]) return new int[3] {1, 2, 3};
                else if (array[2] > array[0]) return new int[3] {3, 1, 2};
                return new int[3] { 1, 3, 2 };
            }
            else
            {
                if (array[0] > array[2]) return new int[3] { 2, 1, 3 };
                else if (array[2] > array[1]) return new int[3] { 3, 2, 1 };
                return new int[3] { 2, 3, 1 };
            }
        }
        else
        {
            if (array[0] == array[1])
            {
                if (array[0] > array[2])
                {
                    return new int[3] { matchWinner[2], matchLoser[2], 3 };
                }
                else
                {
                    return new int[3] { 3, matchWinner[2] , matchLoser[2]};
                }
            }
            else if (array[1] == array[2])
            {
                if (array[1] > array[0])
                {
                    return new int[3] { matchWinner[0], matchLoser[0], 1 };
                }
                else
                {
                    return new int[3] { 1, matchWinner[0], matchLoser[0]};
                }
            }
            else
            {
                if (array[0] > array[1])
                {
                    return new int[3] { matchWinner[1], matchLoser[1], 2 };
                }
                else
                {
                    return new int[3] { 2, matchWinner[1], matchLoser[1] };
                }
            }
        }
    }

    private void InitializePlayers()
    {
        foreach (Player player in players.playerList)
        {            
            if (!player.NPC)
            {
                player.olympicSeed = 0;
                if (player.rank == "First") first = player;
                else if (player.rank == "Second") second = player;
                else third = player;
            }
        }
    }

}
