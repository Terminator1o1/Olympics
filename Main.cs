using RPG.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Random = UnityEngine.Random;

public class Main : MonoBehaviour, ISaveable
{
    [SerializeField] SavingWrapper savingWrapper;
    [SerializeField] Championship championship;
    [SerializeField] WorldCup worldCup;
    [SerializeField] Leaderboards leaderboards;
    [SerializeField] Texture2D cursor;
    [SerializeField] Soundtrack soundtrack;
    [SerializeField] Players players;

    bool playedWorldCup = false; //make sure is saved or use something else!
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Canvas>().enabled = true;
        Cursor.SetCursor(cursor, new Vector2(0,0), CursorMode.ForceSoftware);
        soundtrack.PlaySoundtrack();
    }

    public void StartSession()
    {
        foreach (Player player in players.playerList)
        {
            int offset = CalculateForm();
            //print("form: " + offset);
            player.goalScorePercentage = Mathf.Max(player.baseGoalScorePercentage + offset, 5);
            player.penaltyScorePercentage = Mathf.Min(player.basePenaltyScorePercentage + offset, 95);
        }
        GetComponent<Canvas>().enabled = false;
        GetComponentInChildren<VideoPlayer>().Stop();
        //soundtrack.StopSoundtrack();
        int numOlympicsPlayed = GetNumOlympicsPlayed();
        if ((numOlympicsPlayed % 5 == 0 && !playedWorldCup && numOlympicsPlayed != 0))
        {
            worldCup.StartWorldCup();
        }
        else
        {
            championship.StartChampionship();
        }        
    }

    public static int CalculateForm()
    {
        int num = Random.Range(1, 101);
        if (num <= 10)
            return 0;
        else if (num <= 18)
            return 1;
        else if (num <= 26)
            return -1;
        else if (num <= 34)
            return 2;
        else if (num <= 42)
            return -2;
        else if (num <= 50)
            return 3;
        else if (num <= 58)
            return -3;
        else if (num <= 63)
            return 4;
        else if (num <= 68)
            return -4;
        else if (num <= 73)
            return 5;
        else if (num <= 78)
            return -5;
        else if (num <= 81)
            return 6;
        else if (num <= 84)
            return -6;
        else if (num <= 87)
            return 7;
        else if (num <= 90)
            return -7;
        else if (num <= 92)
            return 8;
        else if (num <= 94)
            return -8;
        else if (num <= 96)
            return 9;
        else if (num <= 98)
            return -9;
        else if (num <= 99)
            return 10;
        else // (num = 100)
            return -10;
    }

    private int GetNumOlympicsPlayed()
    {
        int numOlympicsPlayed = 0;
        foreach (Player player in players.playerList)
        {
            if (!player.NPC)
            {
                numOlympicsPlayed += player.championshipWins;
            }
        }
        return numOlympicsPlayed;
    }

    public void LeaderBoards()
    {
        GetComponent<Canvas>().enabled = false;
        leaderboards.HandleWorldCup();
        leaderboards.GetComponent<Canvas>().enabled = true;
        leaderboards.UpdateCaller(0);
    }

    public void RestartMain(bool worldCup)
    {
        playedWorldCup = worldCup;
        savingWrapper.SaveFile();
        GetComponent<Canvas>().enabled = true;
        GetComponentInChildren<VideoPlayer>().Play();
        soundtrack.PlaySoundtrack();
    }
    public void Quit()
    {
        Application.Quit();
    }

    public object CaptureState()
    {
        return playedWorldCup;
    }

    public void RestoreState(object state)
    {
        playedWorldCup = (bool)state;
    }
}
