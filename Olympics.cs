using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Olympics : MonoBehaviour
{
    [SerializeField] Players players;
    [SerializeField] Nations nations;
    [SerializeField] Match match;
    [SerializeField] Qualifiers qualifiers;
    [SerializeField] Game game;
    [SerializeField] texts[] names;
    [SerializeField] texts[] scores;
    [SerializeField] images[] flags;
    [SerializeField] seeds[] mySeeds;
    [SerializeField] Button proceed;
    [SerializeField] OlympicCeremony olympicCeremony;
    [SerializeField] Leaderboards leaderboards;
    [SerializeField] Canvas firstRoundCanvas;
    [SerializeField] float distributionCooldown;
    [SerializeField] firstRoundRow[] firstRoundRows;
    [SerializeField] Texture2D transparent;
    [SerializeField] Text[] round1seeds;
    [SerializeField] Soundtrack soundtrack;

    int numRound = 1;
    [SerializeField] Player[] playerList;
    Player[] semiFinalists;
    Player[] semiWinners;
    Player[] semiLosers;
    Player bronze;
    Player silver;
    Player gold;
    bool distributing = false;
    bool distributingResults = false;
    int numDistributed = 0;
    bool TreeAllNPC = false;
    //bool RemainingAllNPC = false;

    int numResultsDisplayed = 0;

    [System.Serializable]
    public class texts
    {
        [SerializeField] public Text[] textBoxes;
    }
    [System.Serializable]
    public class images
    {
        [SerializeField] public RawImage[] flagBoxes;
    }

    [System.Serializable]
    public class seeds
    {
        [SerializeField] public Text[] textBoxes;
    }

    [System.Serializable]

    public class firstRoundRow
    {
        [SerializeField] public RawImage flag;
        [SerializeField] public Text name;
        [SerializeField] public Text score;
    }
    public void StartOlympics()
    {
        ClearTexts(names);
        ClearTexts(scores);
        ClearSeeds(mySeeds);
        ClearImages(flags);
        ClearFirstRows();
        playerList = new Player[32];
        numRound = 1;
        TreeAllNPC = false;
        //RemainingAllNPC = false;
        //Player[] playerlist = players.playerList;
        //playerList.
        //bool firstTime = true;
        int i = 0;
        foreach (Player player in players.playerList)
        {
            if (player.status == QualifyingStatus.AutoQualifier)
            {
                players.IncrementTimesQualified(player);
                playerList[i] = player;
                i++;
                player.olympicSeed = 0;
            }
        }

        int[] playerRatings = new int[playerList.Length];
        for (int j = 0; j < playerList.Length; j++)
        {
            playerRatings[j] = playerList[j].rating;
        }
        Array.Sort(playerRatings);
        Array.Reverse(playerRatings);

        Player[] Top8 = new Player[8];

        for (int j = 0; j < 8; j++)
        {
            List<Player> list = new List<Player>();
            foreach (Player player in playerList)
            {
                if (player.rating == playerRatings[j]) list.Add(player);
            }
            foreach (Player player in list)
            {                
                if (j >= 8) break;
                Top8[j] = player;
                player.olympicSeed = j + 1;
                j++;
            }
            j--;
        }

        print("There is the top 8 ");

        foreach (Player player in Top8)
        {
            print(player.name + player.rating);
        }

        int thirdFourth = UnityEngine.Random.Range(2, 4);
        Queue<int> used = new Queue<int>();
        while (used.Count < 4)
        {
            int a = UnityEngine.Random.Range(4, 8);
            if (!used.Contains(a)) used.Enqueue(a);
        }
        int[] apperances = new int[8] { 0, used.Dequeue(), thirdFourth, used.Dequeue(), used.Dequeue(), 5 - thirdFourth, used.Dequeue(), 1};
        Player[] TempPlayerList = new Player[32];

        int next = 0;
        for (int j = 0; j < 32; j++)
        {
            if (j % 8 == 0 || j % 8 == 7)
            {
                TempPlayerList[j] = Top8[apperances[next]];
                next++;
            }
            else
            {
                int a = UnityEngine.Random.Range(0, 32);
                while (InArray(TempPlayerList, playerList[a]) || InArray(Top8, playerList[a]))
                {
                    a = UnityEngine.Random.Range(0, 32);
                }
                TempPlayerList[j] = playerList[a];
            }
        }

        playerList = TempPlayerList;

        DisplayDistribution();
    }

    private void ClearSeeds(seeds[] mySeeds)
    {
        for (int i = 0; i < mySeeds.Length; i++)
        {
            for (int j = 0; j < mySeeds[i].textBoxes.Length; j++)
            {
                mySeeds[i].textBoxes[j].GetComponent<Text>().text = "";
            }
        }
        for (int i = 0; i < 32; i++)
        {
            round1seeds[i].text = "";
        }
    }

    private bool InArray(Player[] topQuarterRated, Player player)
    {
        foreach (Player myplayer in topQuarterRated)
        {
            if (myplayer == player) return true;
        }
        return false;
    }

    public void FirstRoundProceed()
    {
        if (numRound == 1)
        {
            if (distributing)
            {
                distributing = false;
                FinishDistribution();
            }
            else
            {
                firstRoundCanvas.GetComponent<Canvas>().enabled = false;
                GetComponent<AudioSource>().Stop();
                ExecuteRound();
            }
        }
        else
        {           
            TreeAllNPC = true;
            foreach (Player player in playerList)
            {
                if (!player.NPC)
                {
                    TreeAllNPC = false;
                    break;
                }
            }
            if (distributingResults)
            {
                FinishDistributingResults();
            }
            else if (TreeAllNPC)
            {
                firstRoundCanvas.GetComponent<Canvas>().enabled = false;
                print("Skipping tree");
                ExecuteRound();
            }
            else
            {
                firstRoundCanvas.GetComponent<Canvas>().enabled = false;
                GetComponent<Canvas>().enabled = true;
            }
        }
    }

    private void ClearFirstRows() //complete implementation
    {
        foreach (firstRoundRow row in firstRoundRows)
        {
            row.flag.texture = null;
            row.name.text = "";
            row.score.text = "";
        }
    }

    private void FinishDistribution()
    {
        for (int i = 0; i < 32; i++)
        {
            firstRoundRows[i].flag.texture = nations.GetNation(playerList[i].nationality).flag;
            firstRoundRows[i].name.text = playerList[i].name;
            if (playerList[i].olympicSeed > 0) round1seeds[i].text = "#" + playerList[i].olympicSeed;
        }
    }

    private void DisplayDistribution()
    {
        firstRoundCanvas.GetComponent<Canvas>().enabled = true;
        //GetComponent<AudioSource>().Play();
        soundtrack.PlaySoundtrack();
        distributing = true;
        numDistributed = 0;
        for (int i = 0; i < 32; i++)
        {
            Invoke("Distribute", (i + 1) * distributionCooldown);
        }
    }

    private void Distribute()
    {
        if (distributing)
        {
            firstRoundRows[numDistributed].flag.texture = nations.GetNation(playerList[numDistributed].nationality).flag;
            firstRoundRows[numDistributed].name.text = playerList[numDistributed].name;
            if (playerList[numDistributed].olympicSeed > 0) round1seeds[numDistributed].text = "#" + playerList[numDistributed].olympicSeed;
            numDistributed++;
            if (numDistributed == 32) distributing = false;
        }        
    }

    private void ClearTexts(texts[] arr) //complete function
    {
        for (int i = 0; i < arr.Length; i++)
        {
            for (int j = 0; j < arr[i].textBoxes.Length; j++)
            {
                arr[i].textBoxes[j].GetComponent<Text>().text = "";
            }
        }
    }

    private void ClearImages(images[] arr) //complete function
    {
        for (int i = 0; i < arr.Length; i++)
        {
            for (int j = 0; j < arr[i].flagBoxes.Length; j++)
            {
                arr[i].flagBoxes[j].GetComponent<RawImage>().texture = transparent;
            }
        }

    }

    public void ExecuteRound()
    {
        //if (!RemainingAllNPC) soundtrack.StopSoundtrack();
        Node initialNode = null;
        Node prevNode = null;
        //print("a");
        for (int i = 0; i < playerList.Length; i = i + 2)
        {
            Text[] texts = null;
            RawImage[] images = null;
            if (numRound >= 2)
            {
                int j = numRound - 1;
                if (numRound > 4)
                {
                    j++;
                    texts = new Text[3] { scores[numRound - 2].textBoxes[i], scores[numRound - 2].textBoxes[i + 1], names[j].textBoxes[i / 2]};
                    images = new RawImage[1] { flags[j].flagBoxes[i / 2] };
                }
                else if (numRound == 4)
                {
                    texts = new Text[4] { scores[numRound - 2].textBoxes[i], scores[numRound - 2].textBoxes[i + 1], names[j + 1].textBoxes[i / 2], names[j].textBoxes[i / 2], };
                    images = new RawImage[2] { flags[j + 1].flagBoxes[i / 2], flags[j].flagBoxes[i / 2] };
                }
                else
                {
                    texts = new Text[3] { scores[numRound - 2].textBoxes[i], scores[numRound - 2].textBoxes[i + 1], names[j].textBoxes[i / 2], };
                    images = new RawImage[1] { flags[j].flagBoxes[i / 2] };
                }
                //if (playerList[i].olympicSeed > 0) mySeeds[numRound - 2].textBoxes[i].text = "#" + playerList[i].olympicSeed;
                //else mySeeds[numRound - 2].textBoxes[i].text = "";
                //if (playerList[i + 1].olympicSeed > 0) mySeeds[numRound - 2].textBoxes[i + 1].text = "#" + playerList[i + 1].olympicSeed;
                //else mySeeds[numRound - 2].textBoxes[i + 1].text = "";
            }
            else if (numRound == 1)
            {
                texts = new Text[3] {firstRoundRows[i].score, firstRoundRows[i + 1].score , names[0].textBoxes[i / 2] };
                images = new RawImage[1] { flags[0].flagBoxes[i / 2] };
            }            
            Node newNode = new Node(playerList[i], playerList[i + 1], null, match, null, game, this, nations, texts, images, numRound + 2, players, playerList[i].olympicSeed, playerList[i+1].olympicSeed, soundtrack, null, null); //change           
            //Do seeds for round1 here.
            if (initialNode == null) initialNode = newNode;
            else prevNode.nextNode = newNode;
            prevNode = newNode;
            //print("b");
        }
        //print("c");
        initialNode.RunNode(new List<Player>());
        //turn off canvas
    }

    public void AnalyzeNodes(Player[] victors)
    {
        //if (!RemainingAllNPC)
        //{
        //    RemainingAllNPC = true;
        //    foreach (Player victor in victors)
        //    {
        //        if (!victor.NPC)
        //        {
        //            RemainingAllNPC = false;
        //            break;
        //        }
        //    }
        //}
        soundtrack.PlaySoundtrack();
        //turn on canvas
        //print("Analyzing nodes");       
        numRound++;
        DistributeResults();
        playerList = victors;
        if (numRound == 4) semiFinalists = victors;
        else if (numRound == 5)
        {
            semiWinners = victors;
            semiLosers = new Player[2];
            int index = 0;
            foreach (Player player in semiFinalists)
            {
                if (victors[0] != player && victors[1] != player)
                {
                    semiLosers[index] = player;
                    index++;
                }
            }
            playerList = semiLosers;
            //ExecuteRound();
            //return;
        }
        else if (numRound == 6)
        {
            bronze = victors[0];
            foreach (Player player in semiLosers)
            {
                if (player != bronze) player.currentContext = "Semi Finals";
            }
            playerList = semiWinners;
            //ExecuteRound();
            //return;
        }
        else if (numRound == 7)
        {
            gold = victors[0];
            gold.context = "Gold";
            players.IncrementBronzeMedals(bronze);
            if (semiWinners[0] == gold) silver = semiWinners[1];
            else silver = semiWinners[0];
            print("Incrementing Silver");
            players.IncrementSilverMedals(silver);
            print("Incrementing Gold");
            players.IncrementGoldMedals(gold);
            //confirmOlympics.gameObject.SetActive(true);         
            players.UpdateContexts();
            int olympic_num = 0;
            foreach (Player player in players.playerList)
            {
                if (!player.NPC) olympic_num += player.championshipWins;
            }
            gold.acheivments[olympic_num] = "G";

            if (TreeAllNPC) ConfirmOlympics();
            return;
        }        
        //if (numRound < 3) return;
        if (numRound == 2) firstRoundCanvas.GetComponent<Canvas>().enabled = true;
        if (numRound < 5)
        {
            for (int i = 0; i < victors.Length; i++)
            {
                if (victors[i].olympicSeed > 0) mySeeds[numRound - 2].textBoxes[i].text = "#" + victors[i].olympicSeed;
            }
        }
        else if (numRound == 5)
        {
            for (int i = 0; i < victors.Length; i++)
            {
                if (victors[i].olympicSeed > 0) mySeeds[4].textBoxes[i].text = "#" + victors[i].olympicSeed;
            }
            for (int i = 0; i < semiLosers.Length; i++)
            {
                if (semiLosers[i].olympicSeed > 0) mySeeds[3].textBoxes[i].text = "#" + semiLosers[i].olympicSeed;
            }
        }
        if (TreeAllNPC) ExecuteRound();
    }

    private void DistributeResults()
    {
        distributingResults = true;
        numResultsDisplayed = 0;
        print(numRound);
        Text[] texts;
        if (numRound > 2)
        {
            foreach (Text text in scores[numRound - 3].textBoxes)
            {
                text.gameObject.SetActive(false);
                print(text.GetComponent<Text>().enabled);
                print(text.text);
                print("DISABLING TEXT BOX BRO");
            }
            if (numRound < 6)
            {
                foreach (Text text in names[numRound - 2].textBoxes)
                {
                    text.gameObject.SetActive(false);
                }
                foreach (RawImage rawImage in flags[numRound - 2].flagBoxes)
                {
                    rawImage.gameObject.SetActive(false);
                }
                foreach (Text text in mySeeds[numRound - 2].textBoxes)
                {
                    text.gameObject.SetActive(false);
                }
            }
            if (numRound > 4)
            {
                foreach (Text text in names[numRound - 1].textBoxes)
                {
                    text.gameObject.SetActive(false);
                }
                foreach (RawImage rawImage in flags[numRound - 1].flagBoxes)
                {
                    rawImage.gameObject.SetActive(false);
                }
                if (numRound == 5)
                {
                    foreach (Text text in mySeeds[numRound - 2].textBoxes)
                    {
                        text.gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            foreach (firstRoundRow firstRoundRow in firstRoundRows)
            {
                firstRoundRow.score.gameObject.SetActive(false);
                print("DISABLING TEXT BOX BRO");
            }
        }
        Invoke("DisplayResult", distributionCooldown);
    }

    public void DisplayResult()
    {
        if (numRound > 2)
        {
            if (!distributingResults || numResultsDisplayed >= scores[numRound - 3].textBoxes.Length)
            {
                distributingResults = false;
                print("setting false");
                return;
            }
            scores[numRound - 3].textBoxes[numResultsDisplayed].gameObject.SetActive(true);
            scores[numRound -3].textBoxes[numResultsDisplayed + 1].gameObject.SetActive(true);
            if (numRound < 6)
            {
                names[numRound - 2].textBoxes[numResultsDisplayed / 2].gameObject.SetActive(true);
                flags[numRound - 2].flagBoxes[numResultsDisplayed / 2].gameObject.SetActive(true);
                mySeeds[numRound - 2].textBoxes[numResultsDisplayed / 2].gameObject.SetActive(true);
            }
            if (numRound > 4)
            {
                names[numRound - 1].textBoxes[numResultsDisplayed / 2].gameObject.SetActive(true);
                flags[numRound - 1].flagBoxes[numResultsDisplayed / 2].gameObject.SetActive(true);
                if (numRound < 6) mySeeds[numRound - 1].textBoxes[numResultsDisplayed / 2].gameObject.SetActive(true);
            }
            numResultsDisplayed += 2;
            if (numResultsDisplayed >= scores[numRound - 3].textBoxes.Length)
            {
                distributingResults = false;
            }
            else
            {
                Invoke("DisplayResult", distributionCooldown);
            }            
        }
        else
        {
            if (!distributingResults || numResultsDisplayed >= 32)
            {
                distributingResults = false;
                print("setting false");
                return;
            }
            firstRoundRows[numResultsDisplayed].score.gameObject.SetActive(true);
            firstRoundRows[numResultsDisplayed + 1].score.gameObject.SetActive(true);           
            numResultsDisplayed += 2;
            if (numResultsDisplayed >= 32)
            {
                distributingResults = false;
            }
            else
            {
                Invoke("DisplayResult", distributionCooldown);
            }           
        }
    }

    public void ConfirmOlympics()
    {
        GetComponent<Canvas>().enabled = false;
        players.UpdateRatings(true);
        //foreach (Player player in players.playerList)
        //{
        //    player.currentContext = "None";
        //}
        leaderboards.UpdateCaller(1);
        leaderboards.DisplayLeaderBoard("Goals");        
    }

    public void ProceedToCeremony()
    {
        olympicCeremony.StartCeremony(gold, silver, bronze);
    }

    public void Proceed()
    {
        if (numRound == 7) ConfirmOlympics();
        else if (distributingResults) FinishDistributingResults();
        else ExecuteRound();
    }

    private void FinishDistributingResults()
    {
        print("Finishing...");
        if (numRound > 2)
        {
            for (int i = 0; i < scores[numRound - 3].textBoxes.Length; i = i + 2)
            {
                scores[numRound - 3].textBoxes[i].gameObject.SetActive(true);
                scores[numRound - 3].textBoxes[i + 1].gameObject.SetActive(true);
                if (numRound < 6)
                {
                    names[numRound - 2].textBoxes[i / 2].gameObject.SetActive(true);
                    flags[numRound - 2].flagBoxes[i / 2].gameObject.SetActive(true);
                    mySeeds[numRound - 2].textBoxes[i / 2].gameObject.SetActive(true);
                }
                if (numRound > 4)
                {
                    names[numRound - 1].textBoxes[i / 2].gameObject.SetActive(true);
                    flags[numRound - 1].flagBoxes[i / 2].gameObject.SetActive(true);
                    if (numRound < 6) mySeeds[numRound - 1].textBoxes[i / 2].gameObject.SetActive(true);
                }
            }
        }
        else
        {
            for (int i = 0; i < firstRoundRows.Length; i = i + 2)
            {
                firstRoundRows[i].score.gameObject.SetActive(true);
                firstRoundRows[i + 1].score.gameObject.SetActive(true);
            }
        }        
        distributingResults = false;
    }
}