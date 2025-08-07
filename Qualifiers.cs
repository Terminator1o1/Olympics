using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;
using Random = System.Random;

public class Qualifiers : MonoBehaviour
{
    [SerializeField] Nations nations;
    [SerializeField] Match match;
    [SerializeField] Game game;
    [SerializeField] Olympics olympics;
    [SerializeField] Players playersObject;
    [SerializeField] public List<Player> strongCandidates;
    [SerializeField] public List<Player> weakCandidates;
    [SerializeField] int numStrongCandidates; //necessary?
    [SerializeField] int numWeakCandidates;
    [SerializeField] int numRounds = 0;
    [SerializeField] VideoPlayer video;
    [SerializeField] Canvas videoCanvas;

    bool playingVideo = false;
    void Start()
    {
        //InitializeInts();
    }

    private void Update()
    {
        //if (playingVideo && Input.GetMouseButtonDown(0))
        //{
        //    video.Stop();
        //    StartOlympics();
        //}
    }
    public void RunQualifiers()
    {
        numRounds = 0;
        //Player[] strongList = strongCandidates.ToArray();
        Player[] weakList = weakCandidates.ToArray();
        CutByHalf(weakList);
    }

    private void CutByHalf(Player[] players)
    {
        //int index = 0;
        //Player[] arr = new Player[players.Length / 2];
        Node startNode = null;
        Node node = null;

        int[] playerRatings = new int[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playerRatings[i] = players[i].rating;
            players[i].olympicSeed = 0;
        }
        Array.Sort(playerRatings);
        Array.Reverse(playerRatings);

        Player[] TopQuarterRated = new Player[players.Length / 4];

        for (int i = 0; i < players.Length/4; i++)
        {
            List<Player> list = new List<Player>();
            foreach (Player player in players)
            {
                if (player.rating == playerRatings[i]) list.Add(player);
            }
            foreach (Player player in list)
            {
                if (i >= players.Length / 4) break;
                TopQuarterRated[i] = player;
                player.olympicSeed = i + 1;
                i++;
            }
            i--;
        }

        print("There is your Top Quarter players of length " + TopQuarterRated.Length);
        foreach (Player player in TopQuarterRated) print(player.name + player.rating); //should probably remove later

        List<Player> myPlayers = new List<Player>();
        for (int i = 0; i < TopQuarterRated.Length; i++)
        {
                int j = UnityEngine.Random.Range(0, players.Length);
                while (TopQuarterRated[i] == players[j] || myPlayers.Contains(players[j]) || InArray(TopQuarterRated, players[j]))
                {
                    j = UnityEngine.Random.Range(0, players.Length);
                }
            Node newNode = new Node(players[j], TopQuarterRated[i], null, match, this, game, null, nations, null, null, numRounds + 1, playersObject, 0, i + 1, null, null, null);
            print("Matched " + players[j].name + " with " + TopQuarterRated[i].name);
            if (i != 0) node.nextNode = newNode;
            else startNode = newNode;
            node = newNode;
            myPlayers.Add(players[j]);
            myPlayers.Add(TopQuarterRated[i]);
        }
        Player[] NewPlayerList = new Player[players.Length / 2];

        int index = 0;
        foreach (Player player in players)
        {
            if (!myPlayers.Contains(player))
            {
                NewPlayerList[index] = player;
                index++;
            }
        }

        print("index should be " + players.Length / 2 + "and it is: " + index);

        List<int> ints = new List<int>();
        for (int i = 0; i < NewPlayerList.Length; i++)
        {
            if (!ints.Contains(i))
            {
                int j = UnityEngine.Random.Range(0, NewPlayerList.Length);
                while (j == i || ints.Contains(j))
                {
                    j = UnityEngine.Random.Range(0, NewPlayerList.Length);
                }
                Node newNode = new Node(NewPlayerList[i], NewPlayerList[j], null, match, this, game, null, nations, null, null, numRounds + 1, playersObject, 0 ,0, null, null, null);
                print("Matched " + NewPlayerList[i].name + " with " + NewPlayerList[j].name);
                node.nextNode = newNode;
                node = newNode;
                ints.Add(i);
                ints.Add(j);
            }
        }
        startNode.RunNode(new List<Player>());
    }

    private bool InArray(Player[] topQuarterRated, Player player)
    {
        foreach (Player myplayer in topQuarterRated)
        {
            if (myplayer == player) return true;
        }
        return false;
    }

    public void AnalyzeNodes(List<Player> victors)
    {
        numRounds++;
        if (numRounds == 1)
        {
            Player[] victorsArray = victors.ToArray();
            Player[] strongsArray = strongCandidates.ToArray();
            Player[] secondRoundPlayers = new Player[victorsArray.Length + strongsArray.Length];
            for (int i = 0; i < victorsArray.Length + strongsArray.Length; i++)
            {
                if (i < victorsArray.Length) secondRoundPlayers[i] = victorsArray[i];
                else secondRoundPlayers[i] = strongsArray[i - victorsArray.Length];
            }
            CutByHalf(secondRoundPlayers);
        }
        else if (numRounds == 2)
        {
            foreach (Player player in victors)
            {
                player.status = QualifyingStatus.AutoQualifier;
            }
            videoCanvas.GetComponent<Canvas>().enabled = true;
            video.Play();
            playingVideo = true;
            Invoke("StartOlympics", (float)video.length);
            //CutByHalf(strongCandidates.ToArray());
        }
        //else
        //{
        //    foreach (Player player in victors)
        //    {
        //        player.status = QualifyingStatus.AutoQualifier;
        //    }           
        //}
    }

    private void StartOlympics()
    {
        if (playingVideo)
        {
            playingVideo = false;
            videoCanvas.GetComponent<Canvas>().enabled = false;
            ResetData();
            olympics.StartOlympics();
        }       
    }

    private void ResetData() //sussy baka
    {
        strongCandidates.Clear();
        weakCandidates.Clear();
    }

    private void InitializeInts()
    {
        //numStrongCandidates = 0;
        //numWeakCandidates = 0;
        //foreach (Nation nation in nations.nationList)
        //{
        //    numStrongCandidates += nation.numStrongCandidate;
        //    numWeakCandidates += nation.numWeakCandidate;
        //}
        //strongCandidates = new Player[numStrongCandidates];
        //weakCandidates = new Player[numWeakCandidates];
        //print(numStrongCandidates);
        //print(numWeakCandidates);
    }
}