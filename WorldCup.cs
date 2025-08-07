using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class WorldCup : MonoBehaviour
{
    [SerializeField] Nations nations;
    [SerializeField] Players players;
    [SerializeField] Canvas videoCanvas;
    [SerializeField] VideoPlayer video;
    [SerializeField] WorldCupGroup[] groups;
    [SerializeField] WorldCupDraw worldCupDraw;
    [SerializeField] Canvas groupStagesCanvas;
    [SerializeField] Soundtrack soundtrack;
    [SerializeField] float distributionTimeInterval = 5f;
    Nation[] participants;
    Nation[] orderedParticipants;
    bool israelParticipates = false;
    int loaded = 0;
    int numRoundsPlayed = 0;
    bool distributing = false;
    public void StartWorldCup()
    {
        InitializeParticipants();
        InitializeIsraelParticipates();
        ResetStats();
        if (israelParticipates)
        {
            videoCanvas.GetComponent<Canvas>().enabled = true;
            video.Play();
            Invoke("StartDistribution", (float)video.length);
        }
        else
        {
            StartDistribution();
        }
    }

    private void ResetStats()
    {
        foreach(Player player in players.playerList)
        {
            player.olympicSeed = 0;
            player.worldCupGoals = 0;
        }
    }

    private void StartDistribution()
    {
        videoCanvas.GetComponent<Canvas>().enabled = false;
        if (israelParticipates)
        {
            print("israel participates");
            groupStagesCanvas.GetComponent<Canvas>().enabled = true;
            soundtrack.PlaySoundtrack();
        }
            orderedParticipants = new Nation[16];
        for (int i = 0; i < 4; i++)
        {
            int j = 0;
            while (j < 4)
            {
                int offset = UnityEngine.Random.Range(0, 4);
                while (orderedParticipants[i * 4 + offset] != null)
                {
                    offset = UnityEngine.Random.Range(0, 4);
                }
                orderedParticipants[i * 4 + offset] = participants[i * 4 + j];
                j++;
            }          
        }
      
        if (!israelParticipates)
        {
            StartCycle();
        }
        else
        {
            distributing = true;
            int k = 0;
            while (k < 16)
            {
                if (israelParticipates)
                {
                    Invoke("LoadNext", distributionTimeInterval * (k + 1));
                }
                else
                {
                    LoadNext();
                }
                k++;
            }          
        }
    }

    public void GroupsFinished()
    {
        if (!israelParticipates) Proceed();
        else
        {
            soundtrack.PlaySoundtrack();
            groupStagesCanvas.GetComponent<Canvas>().enabled = true;
        }
    }

    public void Proceed()
    {
        if (distributing)
        {
            FinishDistribution();
        }
        else
        {
            if (numRoundsPlayed == 3)
            {
                StartDraw();
            }
            else
            {
                numRoundsPlayed++;
                StartCycle();
            }
        }       
    }

    private void StartDraw()
    {
        groupStagesCanvas.GetComponent<Canvas>().enabled = false;
        Nation[] worldCupParticipants = new Nation[8];
        for (int i = 0; i < 4; i++)
        {
            worldCupParticipants[2 * i] = nations.GetNation(groups[i].nationSlots[0].name.text);
            worldCupParticipants[7 - (2 * i)] = nations.GetNation(groups[i].nationSlots[1].name.text);
        }
        worldCupDraw.StartDraw(worldCupParticipants);
    }

    private void FinishDistribution()
    {
        int k = 0;
        while (k < 16 && distributing)
        {
            LoadNext();
            k++;
        }
    }

    private void StartCycle()
    {
        soundtrack.StopSoundtrack();
        groups[0].PlayRound();
    }

    private void LoadNext()
    {
        if (distributing)
        {
            if (loaded < 16)
            {
                groups[loaded % 4].nationSlots[loaded / 4].LoadNation(orderedParticipants[loaded], true);
                loaded++;
            }
            else
            {
                distributing = false;
            }
        }       
    }

    private void InitializeIsraelParticipates()
    {
        israelParticipates = false;
        foreach (Nation nation in participants)
        {
            if (nation.name == "Israel")
            {
                israelParticipates = true;
                break;
            }
        }
    }

    private void InitializeParticipants()
    {
        participants = new Nation[16];
        Nation[] nonInferiorNations = new Nation[48];
        int k = 0;
        foreach (Nation nation in nations.nationList)
        {
            if (!nation.InferiorCountry)
            {
                nonInferiorNations[k] = nation;
                k++;
                if (k == 48) break;
            }
        }
        int[] ratings = new int[48];
        for (int j = 0; j < 48; j++)
        {
            ratings[j] = nonInferiorNations[j].rating;
        }
        Array.Sort(ratings);
        Array.Reverse(ratings);
        for (int i = 0; i < 16; i++) //can alter conditions for the back of the array
        {
            int j = i;
            if (ratings[i] == 0)
            {
                print("breaking");
                break;
            }
            //rows[i].rank.text = "#" + i;
            Nation[] list = GetNationsRatingBased(ratings[i], nonInferiorNations);
            if (list.Length == 0)
            {
                print("empty list");
                return;
            }
            i--;
            foreach (Nation nation in list)
            {
                i++;
                if (i > 15) break;
                participants[i] = nation;
            }
        }
    }

    private Nation[] GetNationsRatingBased(int v, Nation[] nonInferiorNations)
    {
        List<Nation> resultNations = new List<Nation>();
        foreach (Nation nation in nonInferiorNations)
        {
            if (nation.rating == v)
            {
                resultNations.Add(nation);
            }
        }
        return resultNations.ToArray();
    }
}
