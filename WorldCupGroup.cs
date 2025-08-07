using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldCupGroup : MonoBehaviour
{
    [SerializeField] public NationSlot[] nationSlots;
    [SerializeField] WorldCupGroup nextGroup = null;
    [SerializeField] WorldCup worldCup;
    [SerializeField] Nations nations;
    [SerializeField] Players players;
    [SerializeField] Soundtrack soundtrack;
    [SerializeField] Game game;
    int numRound = 0;
    Nation nation1;
    Nation nation2;
    Nation nation3;
    Nation nation4;
    bool firstMatch = true;
    Nation firstNation;
    Nation secondNation;
    public void PlayRound()
    {
        if (numRound == 0)
        {
            PlayMatch(0, 1);            
        }
        else if (numRound == 1)
        {
            PlayMatch(0, 2);           
        }
        else if (numRound == 2)
        {
            PlayMatch(0, 3);           
        }
        //ConcludeRound();
    }

    private void ConcludeRound()
    {
        numRound++;
        RearrangeSlots();
        if (nextGroup != null)
        {
            nextGroup.PlayRound();
        }
        else
        {
            worldCup.GroupsFinished();
        }
    }

    private void PlayMatch(int nationIndex1, int nationIndex2)
    {
        //firstNation = nations.GetNation("Israel"); //assigment to avoid error
        //secondNation = nations.GetNation("Israel");
        foreach (NationSlot nationSlot in nationSlots)
        {
            if (nations.GetNation(nationSlot.name.text).tier == nationIndex1) firstNation = nations.GetNation(nationSlot.name.text);
            else if (nations.GetNation(nationSlot.name.text).tier == nationIndex2) secondNation = nations.GetNation(nationSlot.name.text);
        }
        if (firstNation == null) print("NULL NATION");
        if (secondNation == null) print("NULL NATION");
        Player[] firstNationTop3 = GetTop3(firstNation);
        Player[] secondNationTop3 = GetTop3(secondNation);
        Node thirdGame = new Node(firstNationTop3[2], secondNationTop3[2], null, null, null, game, null, nations, null, null, 0, players, 0 ,0 , soundtrack, this, null);
        Node secondGame = new Node(firstNationTop3[1], secondNationTop3[1], thirdGame, null, null, game, null, nations, null, null, 0, players, 0, 0, soundtrack, this, null);
        Node firstGame = new Node(firstNationTop3[0], secondNationTop3[0], secondGame, null, null, game, null, nations, null, null, 0, players, 0, 0, soundtrack, this, null);
        List<Player> victors = new List<Player>();
        firstGame.RunNode(victors);
    }

    internal void AnalyzeNodes(Player[] victors)
    {
        int firstNationWins = 0;
        int secondNationWins = 0;
        foreach (Player player in victors)
        {
            if (player.nationality == firstNation.name) firstNationWins++;
            else secondNationWins++;
        }
        firstNation.worldCupGroupStagesWins += firstNationWins;
        secondNation.worldCupGroupStagesWins += secondNationWins;
        if (firstNationWins > secondNationWins) firstNation.worldCupPoints++;
        else secondNation.worldCupPoints++;
        if (firstMatch)
        {
            firstMatch = false;
            if (numRound == 0)
            {
                PlayMatch(2, 3);
            }
            else if (numRound == 1)
            {
                PlayMatch(1, 3);
            }
            else if (numRound == 2)
            {
                PlayMatch(1, 2);
            }
        }
        else
        {
            firstMatch = true;
            ConcludeRound();
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

    private void RearrangeSlots()
    {
        Nation[] groupNations = new Nation[4];
        int[] nationSortingPoints = new int[4];
        int k = 0;
        foreach (NationSlot nationSlot in nationSlots)
        {
            Nation nation = nations.GetNation(nationSlot.name.text);
            nation.worldCupSortingPoints = 0;
            nation.worldCupSortingPoints += nation.rating;
            nation.worldCupSortingPoints += 100000 * nation.worldCupGroupStagesGoals;
            nation.worldCupSortingPoints += 10000000 * nation.worldCupGroupStagesWins;
            nation.worldCupSortingPoints += 100000000 * nation.worldCupPoints;
            groupNations[k] = nation;
            nationSortingPoints[k] = nation.worldCupSortingPoints;
            k++;
        }
        Array.Sort(nationSortingPoints);
        Array.Reverse(nationSortingPoints);
        for (int i = 0; i < 4; i++) //can alter conditions for the back of the array
        {
            int j = i;
            //if (nationSortingPoints[i] == 0)
            //{
            //    print("breaking");
            //    break;
            //}
            //rows[i].rank.text = "#" + i;
            Nation[] list = GetNationsBySortingPoints(groupNations, nationSortingPoints[i]);
            if (list.Length == 0)
            {
                print("empty list");
                return;
            }
            i--;
            foreach (Nation nation in list)
            {
                i++;
                nationSlots[i].LoadNation(nation, false);
            }
        }
    }

    private Nation[] GetNationsBySortingPoints(Nation[] groupNations, int nationSortingPoint)
    {
        List<Nation> resultNations = new List<Nation>();
        foreach (Nation nation in groupNations)
        {
            if (nation.worldCupSortingPoints == nationSortingPoint)
            {
                resultNations.Add(nation);
            }
        }
        return resultNations.ToArray();
    }

    [System.Serializable]
    public class NationSlot
    {
        [SerializeField] public Text name;
        [SerializeField] RawImage flag;
        [SerializeField] Text points;
        [SerializeField] Text wins;
        [SerializeField] Text goals;
        [SerializeField] public int tier;
        public void LoadNation(Nation nation, bool initial)
        {
            if (initial)
            {
                nation.tier = tier;
                nation.worldCupPoints = 0;
                nation.worldCupGroupStagesWins = 0;
                nation.worldCupGroupStagesGoals = 0;
            }
            name.text = nation.name;
            flag.texture = nation.flag;
            points.text = nation.worldCupPoints.ToString();
            wins.text = nation.worldCupGroupStagesWins.ToString();
            goals.text = nation.worldCupGroupStagesGoals.ToString();            
        }
    }    
}
