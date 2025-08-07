using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldCupDraw : MonoBehaviour
{
    [SerializeField] Row[] rows;
    [SerializeField] Nations nations;
    [SerializeField] Players players;
    [SerializeField] Game game;
    [SerializeField] Soundtrack soundtrack;
    [SerializeField] Leaderboards leaderboards;
    [SerializeField] WorldCupCeremony worldCupCeremony;
    Nation[] participants;
    bool israelParticipates = false;
    int currentRound = 0;
    int currentGame = 0;
    Nation firstNation;
    Nation secondNation;
    Nation firstPlace;
    Nation secondPlace;
    Nation thirdPlace;

    public void StartDraw(Nation[] participants)
    {
        currentRound = 0;
        this.participants = participants;        
        InitializeParticipants();
        InitializeIsraelParticipates();
        if (israelParticipates) GetComponent<Canvas>().enabled = true;
        else
        {
            Proceed();
        }
    }

    private void InitializeParticipants()
    {
        for (int i = 0; i < participants.Length; i++)
        {
            rows[0].nationSlots[i].LoadNation(participants[i]);
        }
    }

    public void Proceed()
    {
        GetComponent<Canvas>().enabled = false;
        currentRound++;
        if (currentRound == 5)
        {
            firstPlace.worldCupFirst++;
            firstPlace.worldCupTableSortingPoints += 1000000;
            secondPlace.worldCupSecond++;
            secondPlace.worldCupTableSortingPoints += 1000;
            thirdPlace.worldCupThird++;
            thirdPlace.worldCupTableSortingPoints++;
            GetComponent<Canvas>().enabled = false;
            leaderboards.UpdateCaller(3);
            leaderboards.DisplayLeaderBoard("Individual Goals");
        }
        else
        {
            currentGame = 0;
            NationsMatch();
        }       
    }

    public void ConcludeWorldCup()
    {
        worldCupCeremony.StartCeremony(firstPlace, secondPlace, thirdPlace);
    }

    private void NationsMatch()
    {
        firstNation = nations.GetNation(rows[currentRound - 1].nationSlots[currentGame * 2].name.text);
        secondNation = nations.GetNation(rows[currentRound - 1].nationSlots[currentGame * 2 + 1].name.text);
        Player[] firstNationTop3 = GetTop3(firstNation);
        Player[] secondNationTop3 = GetTop3(secondNation);
        Node thirdGame = new Node(firstNationTop3[2], secondNationTop3[2], null, null, null, game, null, nations, null, null, currentRound, players, 0, 0, soundtrack, null, this);
        Node secondGame = new Node(firstNationTop3[1], secondNationTop3[1], thirdGame, null, null, game, null, nations, null, null, currentRound, players, 0, 0, soundtrack, null, this);
        Node firstGame = new Node(firstNationTop3[0], secondNationTop3[0], secondGame, null, null, game, null, nations, null, null, currentRound, players, 0, 0, soundtrack, null, this);
        List<Player> victors = new List<Player>();
        firstGame.RunNode(victors);
    }

    public void AnalyzeNodes(Player[] victors)
    {
        int firstNationWins = 0;
        int secondNationWins = 0;
        foreach (Player victor in victors)
        {
            if (victor.nationality == firstNation.name) firstNationWins++;
            else secondNationWins++;
        }
        rows[currentRound - 1].nationSlots[currentGame * 2].points.text = firstNationWins.ToString();
        rows[currentRound - 1].nationSlots[(currentGame * 2) + 1].points.text = secondNationWins.ToString();
        Nation winner;
        Nation loser;
        if (firstNationWins > secondNationWins)
        {
            winner = firstNation;
            loser = secondNation;
        }
        else
        {
            winner = secondNation;
            loser = firstNation;
        }
        if (currentRound == 1)
        {
            rows[1].nationSlots[currentGame].LoadNation(winner);
        }
        else if (currentRound == 2)
        {
            rows[2].nationSlots[currentGame].LoadNation(loser);
            rows[3].nationSlots[currentGame].LoadNation(winner);
        }
        else
        {
            rows[currentRound + 1].nationSlots[0].LoadNation(winner);
            if (currentRound == 3)
            {
                thirdPlace = winner;
            }
            else
            {
                firstPlace = winner;
                secondPlace = loser;
            }
            
        }
        if (currentGame < rows[currentRound - 1].nationSlots.Length/2 - 1)
        {
            print("A");
            currentGame++;
            NationsMatch();
        }
        else if (israelParticipates)
        {
            print("B");
            GetComponent<Canvas>().enabled = true;
            soundtrack.PlaySoundtrack();
        }
        else
        {
            print("C");
            Proceed();
        }
    }

    private Player[] GetTop3(Nation nation)
    {
        Player[] top3 = new Player[3];
        List<Player> nationPlayers = new List<Player>();
        foreach (Player player in players.playerList)
        {
            if (player.nationality == nation.name)
            {
                nationPlayers.Add(player);
            }
        }
        Player[] nationPlayersArray = nationPlayers.ToArray();
        int[] ratings = new int[nationPlayersArray.Length];
        for (int j = 0; j < nationPlayersArray.Length; j++)
        {
            ratings[j] = nationPlayersArray[j].rating;
        }
        Array.Sort(ratings);
        Array.Reverse(ratings);
        for (int i = 0; i < 3; i++) //can alter conditions for the back of the array
        {
            int j = i;
            //if (ratings[i] == 0)
            //{
            //    print("breaking");
            //    break;
            //}
            //rows[i].rank.text = "#" + i;
            Player[] list = GetPlayersRatingNation(nation, ratings[i]);
            if (list.Length == 0)
            {
                print("empty list");
                return null;
            }
            i--;
            foreach (Player player in list)
            {
                i++;
                if (i > 2) break;
                top3[i] = player;
            }
        }
        return top3;
    }

    private Player[] GetPlayersRatingNation(Nation nation, int rating)
    {
        List<Player> playersList = new List<Player>();
        foreach (Player player in players.playerList)
        {
            if (player.nationality == nation.name && player.rating == rating)
            {
                playersList.Add(player);
            }
        }
        return playersList.ToArray();
    }

    private void InitializeIsraelParticipates()
    {
        israelParticipates = false;
        foreach (Nation nation in participants)
        {
            if (nation.name == "Israel")
            {
                israelParticipates = true;
                return;
            }
        }
    }

    [System.Serializable]
    public class Row
    {
        [SerializeField] public NationSlot[] nationSlots;

        [System.Serializable]
        public class NationSlot
        {
            [SerializeField] public RawImage flag;
            [SerializeField] public Text name;
            [SerializeField] public Text points;
            public void LoadNation(Nation nation)
            {
                flag.texture = nation.flag;
                name.text = nation.name;
            }
        }
    }
}
