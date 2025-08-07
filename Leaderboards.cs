using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Leaderboards : MonoBehaviour
{
    [SerializeField] Canvas leaderboard;
    [SerializeField] Main main;
    [SerializeField] Players players;
    [SerializeField] Nations nations;
    [SerializeField] Row[] rows;
    [SerializeField] Text context;
    [SerializeField] Button mode;
    [SerializeField] Canvas medalsCanvas;
    [SerializeField] Text medalsHeadTitle;
    [SerializeField] Row[] medalRows;
    [SerializeField] Olympics olympics;
    [SerializeField] OlympicCeremony olympicCeremony;
    [SerializeField] WorldCupDraw worldCupDraw;
    [SerializeField] RawImage[] goldenBoots;
    [SerializeField] RawImage goldenBoot;
    [SerializeField] Button returnToLeaderboards;
    [SerializeField] Button medalsReturnToLeaderboards;
    [SerializeField] Texture2D transparent;
    [SerializeField] Canvas historyCanvas;

    [SerializeField] bool allNations = true;
    [SerializeField] HistoryRow[] historyRows;
    [SerializeField] HistoryImageRow[] historyImageRows;
    [SerializeField] Texture2D gold;
    [SerializeField] Texture2D silver;
    [SerializeField] Texture2D bronze;
    [SerializeField] Texture2D SemiFinals;
    [SerializeField] Texture2D QuarterFinals;
    [SerializeField] Texture2D lightBlue;
    [SerializeField] Texture2D white;
    [SerializeField] Text page;
    [SerializeField] RawImage block;
    [SerializeField] Text medalPage;
    [SerializeField] RawImage medalBlock;
    [SerializeField] Text historyPage;
    [SerializeField] Transform worldCupParent;
    int caller = 0;
    int currentPage = 1;
    int currentVerticalPage = 1;
    int tableType = 0;
    //public void DisplayBaseGoalScorePercentageLeaderboard()
    //{
    //    DisplayLeaderBoard(Stats.baseGoalScorePercentage);
    //}

    public void Start()
    {
        
    }
    public void SwitchMode()
    {
        if (allNations)
        {
            mode.GetComponent<Image>().color = Color.cyan;
            mode.GetComponentInChildren<Text>().text = "Users";
        }
        else
        {
            mode.GetComponent<Image>().color = Color.magenta;
            mode.GetComponentInChildren<Text>().text = "All Players";
        }
        allNations = !allNations;       
    }

    public void UpdateCaller(int caller)
    {
        allNations = true;
        mode.GetComponent<Image>().color = Color.magenta;
        mode.GetComponentInChildren<Text>().text = "All Players";
        this.caller = caller;
        if (caller == 0)
        {
            returnToLeaderboards.GetComponentInChildren<Text>().text = "Return";
            medalsReturnToLeaderboards.GetComponentInChildren<Text>().text = "Return";           
        }
        else if (caller == 1) returnToLeaderboards.GetComponentInChildren<Text>().text = "Continue";
        else if (caller == 2) medalsReturnToLeaderboards.GetComponentInChildren<Text>().text = "Continue";
        else if (caller == 3) returnToLeaderboards.GetComponentInChildren<Text>().text = "Continue";
        else if (caller == 4) medalsReturnToLeaderboards.GetComponentInChildren<Text>().text = "Continue";
    }

    public void NextVerticalPage()
    {
        if (currentVerticalPage == 30) return;
        currentVerticalPage++;
        DisplayAppropiateTable();
    }

    private void DisplayAppropiateTable()
    {
        if (tableType == 0) DisplayLeaderBoard(context.text);
        else if (tableType == 1) DisplayPlayerMedalTable();
        else if (tableType == 2) DisplayNationMedalTable(false);
        else if (tableType == 3) DisplayHistoryTablePlayers(currentPage);
        else if (tableType == 4) DisplayNationMedalTable(true);
        else if (tableType == 5) DisplayNationRatings();
    }

    public void PreviousVerticalPage()
    {
        if (currentVerticalPage == 1) return;
        currentVerticalPage--;
        DisplayAppropiateTable();
    }
    public void NextPage()
    {
        int olympic = 0;
        foreach (Player player in players.playerList)
        {
            if (!player.NPC) olympic += player.championshipWins;
        }
        if (olympic >= 1 + ((currentPage) * (historyRows[0].row.Length - 1))) DisplayHistoryTablePlayers(currentPage + 1);
    }

    public void PreviousPage()
    {
        if (currentPage > 1) DisplayHistoryTablePlayers(currentPage - 1); 
    }

    public void DisplayLeaderBoard(String stat)
    {
        tableType = 0;
        ClearLeaderboard();
        leaderboard.GetComponent<Canvas>().enabled = true;
        GetComponent<Canvas>().enabled = false;
        page.text = "Page: " + currentVerticalPage;
        block.GetComponent<RawImage>().enabled = currentVerticalPage != 1;
        context.text = stat;
        Player[] playerList;
        if (allNations) playerList = players.playerList;
        else playerList = players.GetPlayers(0, "baseGoalScorePercentage", false);
        print(playerList.Length);
        int[] stats = new int[playerList.Length];
        int k = 0;
        for (int i = 0; i < playerList.Length; i++)
        {
            Player player = playerList[i];
            int playerStat = players.GetStat(player, stat);
            if (((player.timesQualified >= 5 || stat == "Goals" || stat == "Ratings" || stat == "Olympics at number 1" || stat == "Golden Boot Awards" || stat == "Olympics Qualified" || stat == "Championships Won" || stat == "Championship Finals" || stat == "Group First Places") && (stat != "Penalty Accuracy" || player.OlympicPenaltyScored >= 15) && (stat != "Penalty Win Percentage" || player.PenaltyShootOutVictoriesOlympics >= 5)))
            {
                stats[k] = playerStat;
                k++;
            }               
        }
        int[] newStats = new int[k];
        for (int a = 0; a < k; a++)
        {
            newStats[a] = stats[a];
        }
        stats = newStats;
        print("Stats length is : " + stats.Length);
        Array.Sort(stats);
        Array.Reverse(stats);
        for (int i = 0; i < Mathf.Min(10 * currentVerticalPage, k); i++) //can alter conditions for the back of the array
        {
            int j = i;
            if (stats[i] == 0)
            {
                print("breaking");
                break;
            }
            //rows[i].rank.text = "#" + i;
            Player[] list = players.GetPlayers(stats[i], stat, allNations);
            //bool codeReached = false;
            i--;
            foreach (Player player in list)
            {
                i++;
                if (i >= 10 * currentVerticalPage || i < 10 * (currentVerticalPage - 1))
                {
                    continue;
                }
                //codeReached = true;
                rows[i % 10].flag.texture = nations.GetNation(player.nationality).flag;
                rows[i % 10].name.text = player.name;
                rows[i % 10].name.color = Color.green;
                rows[i % 10].stat_right.text = stats[j].ToString();
            }
            //if (codeReached) i--;
        }
        if (stat == "Goals" || stat == "Individual Goals")
        {
            for (int j = 0; j < 10; j++)
            {
                if (rows[j].stat_right.text == stats[0].ToString()) goldenBoots[j].GetComponent<RawImage>().enabled = true;
                else break;
            }
        }
        goldenBoot.GetComponent<RawImage>().enabled = (stat == "Golden Boot Awards" || stat == "Individual Goals"); 
    }

    public void DisplayPlayerMedalTable()
    {
        tableType = 1;
        ClearLeaderboard();
        medalsCanvas.GetComponent<Canvas>().enabled = true;
        GetComponent<Canvas>().enabled = false;
        medalPage.text = "Page: " + currentVerticalPage;
        medalBlock.GetComponent<RawImage>().enabled = currentVerticalPage != 1;
        medalsHeadTitle.text = "Player Medal Table";
        Player[] playerList;
        if (allNations) playerList = players.playerList;
        else playerList = players.GetPlayers(0, "baseGoalScorePercentage", false);
        int[] stats = new int[playerList.Length];
        for (int i = 0; i < playerList.Length; i++)
        {
            stats[i] = players.GetStat(playerList[i], "Medal Points");
        }
        print("Stats length is : " + stats.Length);
        Array.Sort(stats);
        Array.Reverse(stats);
        for (int i = 0; i < Mathf.Min(10 * currentVerticalPage, playerList.Length); i++) //can alter conditions for the back of the array
        {
            int j = i;
            if (stats[i] == 0)
            {
                print("breaking");
                break;
            }
            //rows[i].rank.text = "#" + i;
            Player[] list = players.GetPlayers(stats[i], "Medal Points", allNations);
            i--;
            foreach (Player player in list)
            {
                i++;
                if (i >= 10 * currentVerticalPage || i < 10 * (currentVerticalPage - 1)) continue;
                medalRows[i % 10].flag.texture = nations.GetNation(player.nationality).flag;
                medalRows[i % 10].name.text = player.name;
                medalRows[i % 10].stat_left.text = player.goldMedals.ToString();
                medalRows[i % 10].stat_middle.text = player.silverMedals.ToString();
                medalRows[i % 10].stat_right.text = player.bronzeMedals.ToString();
                medalRows[i % 10].stat_far_right.text = (player.goldMedals + player.silverMedals + player.bronzeMedals).ToString();              
            }
        }
    }

    public void DisplayHistoryTablePlayers(int page)
    {
        tableType = 3;
        ClearLeaderboard();
        currentPage = page;
        historyCanvas.GetComponent<Canvas>().enabled = true;
        GetComponent<Canvas>().enabled = false;
        historyPage.text = "Page: " + currentVerticalPage;
        medalsHeadTitle.text = "Player History Table";
        Player[] playerList;
        if (allNations) playerList = players.playerList;
        else playerList = players.GetPlayers(0, "baseGoalScorePercentage", false);
        int[] stats = new int[playerList.Length];
        for (int i = 0; i < playerList.Length; i++)
        {
            stats[i] = players.GetStat(playerList[i], "Medal Points");
        }
        print("Stats length is : " + stats.Length);
        Array.Sort(stats);
        Array.Reverse(stats);
        int temp = 1;
        for (int olympic = 1 + ((page - 1) * (historyRows[0].row.Length - 1)); olympic < 1 + ((page) * (historyRows[0].row.Length - 1)); olympic++)
        {
            historyRows[0].row[temp].text = olympic.ToString();
            string bestNonNpcPerformance = GetBestNonNPCPerformance(olympic, playerList);
            if (bestNonNpcPerformance == "G") historyImageRows[0].image[((olympic - 1) % 10) + 1].texture = gold;
            else if (bestNonNpcPerformance == "S") historyImageRows[0].image[((olympic - 1) % 10) + 1].texture = silver;
            else if (bestNonNpcPerformance == "B") historyImageRows[0].image[((olympic - 1) % 10) + 1].texture = bronze;
            else if (bestNonNpcPerformance == "SF") historyImageRows[0].image[((olympic - 1) % 10) + 1].texture = SemiFinals;
            else if (bestNonNpcPerformance == "QF") historyImageRows[0].image[((olympic - 1) % 10) + 1].texture = QuarterFinals;
            else if (bestNonNpcPerformance == "R1" || bestNonNpcPerformance == "R2") historyImageRows[0].image[((olympic - 1) % 10) + 1].texture = lightBlue;
            else historyImageRows[0].image[((olympic - 1) % 10) + 1].texture = white;
            temp++;
        }

        for (int i = 0; i < Mathf.Min(10 * currentVerticalPage, playerList.Length); i++) //can alter conditions for the back of the array
        {
            int j = i;
            //if (stats[i] == 0)
            //{
            //    print("breaking");
            //    break;
            //}
            //rows[i].rank.text = "#" + i;
            Player[] list = players.GetPlayers(stats[i], "Medal Points", allNations);
            i--;
            foreach (Player player in list) 
            {
                i++;
                if (i >= 10 * currentVerticalPage || i < 10 * (currentVerticalPage - 1)) continue;
                historyRows[(i % 10) + 1].row[0].text = player.name;
                if (player.context == "Gold") historyImageRows[(i % 10) + 1].image[0].texture = gold;
                else if (player.context == "Silver") historyImageRows[(i % 10) + 1].image[0].texture = silver;
                else if (player.context == "Bronze") historyImageRows[(i % 10) + 1].image[0].texture = bronze;
                else if (player.context == "Semi Finals") historyImageRows[(i % 10) + 1].image[0].texture = SemiFinals;
                else if (player.context == "Quarter Finals") historyImageRows[(i % 10) + 1].image[0].texture = QuarterFinals;
                else if (player.context == "First Round" || player.context == "Round of 16") historyImageRows[(i % 10) + 1].image[0].texture = lightBlue;
                else historyImageRows[(i % 10) + 1].image[0].texture = white;
                int olympic = int.Parse(historyRows[0].row[1].text);
                for (int square = 1; square < historyRows[(i % 10) + 1].row.Length; square++)
                {
                    string acheivment = player.acheivments[olympic];
                    if (acheivment != null)
                    {
                        historyRows[(i % 10) + 1].row[square].text = acheivment;
                        //historyRows[i + 1].row[square].fontStyle = FontStyle.Bold;

                        //if (acheivment == "G") historyImageRows[i+1].image[square].color = new C

                        if (acheivment == "G") historyImageRows[(i % 10) + 1].image[square].texture = gold;
                        else if (acheivment == "S") historyImageRows[(i % 10) + 1].image[square].texture = silver;
                        else if (acheivment == "B") historyImageRows[(i % 10) + 1].image[square].texture = bronze;
                        else if (acheivment == "SF") historyImageRows[(i % 10) + 1].image[square].texture = SemiFinals;
                        else if (acheivment == "QF") historyImageRows[(i % 10) + 1].image[square].texture = QuarterFinals;
                        else if (acheivment == "R1" || acheivment == "R2") historyImageRows[(i % 10) + 1].image[square].texture = lightBlue;
                        else historyImageRows[(i % 10) + 1].image[square].texture = white;
                        // else historyRows[i + 1].row[square].fontStyle = FontStyle.Normal;
                        olympic++;
                        continue;
                    }
                    else
                    {
                        historyImageRows[(i % 10) + 1].image[square].color = Color.white;
                    }
                    break;
                }
            }
        }
    }

    private string GetBestNonNPCPerformance(int olympic, Player[] playerList)
    {
        int maxOlympicPerformance = 0;
        string[] contextList = new string[3];
        int i = 0;
        foreach (Player player in playerList)
        {
            if (!player.NPC)
            {
                contextList[i] = player.acheivments[olympic];
                i++;
            }
            if (i == 3)
            {
                break;
            }
        }
        for (int j = 0; j < players.contexts.Length; j++)
        {
            if (players.contexts_shorts[j] == contextList[0] || players.contexts_shorts[j] == contextList[1] || players.contexts_shorts[j] == contextList[2])
            {
                maxOlympicPerformance = j;
            }
        }
        return players.contexts_shorts[maxOlympicPerformance];
    }

    public void DisplayNationRatings()
    {
        tableType = 5;
        ClearLeaderboard();
        leaderboard.GetComponent<Canvas>().enabled = true;
        GetComponent<Canvas>().enabled = false;
        page.text = "Page: " + currentVerticalPage;
        block.GetComponent<RawImage>().enabled = currentVerticalPage != 1;
        context.text = "Nation Ratings";
        Nation[] nationList = nations.GetNonInferiors();
        print(nationList.Length);
        int[] stats = new int[nationList.Length];
        int k = 0;
        for (int i = 0; i < nationList.Length; i++)
        {
            Nation nation = nationList[i];
            int nationStat = nation.rating;
            stats[k] = nationStat;
            k++;
        }
        int[] newStats = new int[k];
        for (int a = 0; a < k; a++)
        {
            newStats[a] = stats[a];
        }
        stats = newStats;
        print("Stats length is : " + stats.Length);
        Array.Sort(stats);
        Array.Reverse(stats);
        for (int i = 0; i < Mathf.Min(10 * currentVerticalPage, k); i++) //can alter conditions for the back of the array
        {
            int j = i;
            if (stats[i] == 0)
            {
                print("breaking");
                break;
            }
            //rows[i].rank.text = "#" + i;
            Nation[] list = nations.GetNationsRatings(stats[i]);
            //bool codeReached = false;
            i--;
            foreach (Nation nation in list)
            {
                i++;
                if (i >= 10 * currentVerticalPage || i < 10 * (currentVerticalPage - 1))
                {
                    continue;
                }
                //codeReached = true;
                rows[i % 10].flag.texture = nation.flag;
                rows[i % 10].name.text = nation.name;
                rows[i % 10].stat_right.text = stats[j].ToString();
                if (i < 16) rows[i % 10].name.color = Color.blue;
                else rows[i % 10].name.color = Color.green;
            }
            //if (codeReached) i--;
        }
    }

    public void DisplayNationMedalTable(bool worldCup)
    {
        if (worldCup) tableType = 4;
        else tableType = 2;
        ClearLeaderboard();
        medalsCanvas.GetComponent<Canvas>().enabled = true;
        GetComponent<Canvas>().enabled = false;
        medalPage.text = "Page: " + currentVerticalPage;
        medalBlock.GetComponent<RawImage>().enabled = currentVerticalPage != 1;
        if (worldCup) medalsHeadTitle.text = "Champions League table";
        else medalsHeadTitle.text = "Nation Medal Table";
        Nation[] nationList = nations.nationList;
        int[] stats;
        if (worldCup) stats = new int[48];
        else stats = new int[nationList.Length];
        int k = 0;
        for (int i = 0; i < nationList.Length; i++)
        {
            if (worldCup)
            {
                if (!nationList[i].InferiorCountry)
                {
                    stats[k] = nationList[i].worldCupTableSortingPoints;
                    k++;
                }                
            }
            else stats[i] = nationList[i].medalPoints;
        }
        Array.Sort(stats);
        Array.Reverse(stats);
        for (int i = 0; i < Mathf.Min(10 * currentVerticalPage, stats.Length); i++) //can alter conditions for the back of the array
        {
            int j = i;
            if (stats[i] == 0)
            {
                print("breaking");
                break;
            }
            //rows[i].rank.text = "#" + i;
            Nation[] list;
            if (worldCup) list = nations.GetNationsSortingPoints(stats[i]);
            else list = nations.GetNations(stats[i]);
            if (list.Length == 0)
            {
                print("empty list");
                return;
            }
            i--;
            foreach (Nation nation in list)
            {
                i++;
                if (i >= 10 * currentVerticalPage || i < 10 * (currentVerticalPage - 1)) continue;
                medalRows[i % 10].flag.texture = nation.flag;
                medalRows[i % 10].name.text = nation.name;
                if (worldCup)
                {
                    medalRows[i % 10].stat_left.text = nation.worldCupFirst.ToString();
                    medalRows[i % 10].stat_middle.text = nation.worldCupSecond.ToString();
                    medalRows[i % 10].stat_right.text = nation.worldCupThird.ToString();
                    medalRows[i % 10].stat_far_right.text = (nation.worldCupFirst + nation.worldCupSecond + nation.worldCupThird).ToString();
                }
                else
                {
                    medalRows[i % 10].stat_left.text = nation.gold.ToString();
                    medalRows[i % 10].stat_middle.text = nation.silver.ToString();
                    medalRows[i % 10].stat_right.text = nation.bronze.ToString();
                    medalRows[i % 10].stat_far_right.text = (nation.gold + nation.silver + nation.bronze).ToString();
                }                
            }
        }
    }
    public void ReturnToLeaderboards()
    {
        HandleWorldCup();
        currentVerticalPage = 1;
        leaderboard.GetComponent<Canvas>().enabled = false;
        medalsCanvas.GetComponent<Canvas>().enabled = false;
        historyCanvas.GetComponent<Canvas>().enabled = false;
        if (caller == 0) GetComponent<Canvas>().enabled = true;
        else if (caller == 1)
        {
            for (int i = 0; i < 10; i++)
            {
                if (rows[i].stat_right.text == rows[0].stat_right.text)
                {
                    foreach (Player player in players.playerList)
                    {
                        if (player.name == rows[i].name.text)
                        {
                            players.IncrementGoldenBoots(player);
                            break;
                        }
                    }
                }
                else break;
            }
            olympics.ProceedToCeremony();
        }
        else if (caller == 2 || caller == 4)
        {
            olympicCeremony.GetComponent<AudioSource>().Stop();
            main.RestartMain(caller == 4);
        }
        else if (caller == 3)
        {
            for (int i = 0; i < 10; i++)
            {
                if (rows[i].stat_right.text == rows[0].stat_right.text)
                {
                    foreach (Player player in players.playerList)
                    {
                        if (player.name == rows[i].name.text)
                        {
                            player.worldCupGoldenBoots++;
                            break;
                        }
                    }
                }
                else break;
            }
            worldCupDraw.ConcludeWorldCup();
        }
    }

    public void HandleWorldCup()
    {
        worldCupParent.gameObject.SetActive(false);
        foreach (Nation nation in nations.nationList)
        {
            if (nation.worldCupTableSortingPoints > 0)
            {
                worldCupParent.gameObject.SetActive(true);
                print("Breaking the thing");
                break;
            }
        }
    }

    public void ReturnToMain()
    {
        GetComponent<Canvas>().enabled = false;
        main.GetComponent<Canvas>().enabled = true;
    }

    public void ClearLeaderboard()
    {
        foreach (Row row in rows)
        {
            //row.rank.text = "";
            row.flag.texture = transparent;
            row.name.text = "";
            row.stat_right.text = "";
        }
        foreach (Row row in medalRows)
        {
            row.flag.texture = transparent;
            row.name.text = "";
            row.stat_right.text = "";
            row.stat_far_right.text = "";
            row.stat_left.text = "";
            row.stat_middle.text = "";
        }
        foreach (RawImage boot in goldenBoots)
        {
            boot.GetComponent<RawImage>().enabled = false;
        }
        for (int i = 1; i < historyRows.Length; i++)
        {
            for (int j = 0; j < historyRows[0].row.Length; j++)
            {
                historyRows[i].row[j].text = "";
                historyRows[i].row[j].color = Color.black;
                historyImageRows[i].image[j].texture = white;
            }
        }
    }

    [System.Serializable]
    public class Row 
    {
        //[SerializeField] public Text rank;
        [SerializeField] public RawImage flag;
        [SerializeField] public Text name;
        [SerializeField] public Text stat_right;
        [SerializeField] public Text stat_middle;
        [SerializeField] public Text stat_left;
        [SerializeField] public Text stat_far_right;
    }

    [System.Serializable]
    public class HistoryRow 
    {
        [SerializeField] public Text[] row;
    }
    [System.Serializable]
    public class HistoryImageRow 
    {
        [SerializeField] public RawImage[] image;
    }

}
