using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locals : MonoBehaviour
{
    [SerializeField] Nations nations;
    [SerializeField] Players players;
    [SerializeField] List<Player> inferiorCandidates;

    public void RunLocals(Player first, Player second, Player third)
    {
        inferiorCandidates = new List<Player>();
        foreach (Nation nation in nations.nationList)
        {
            if (nation.name != "Israel")
            {
                List<Player> playerlist = new List<Player>();
                foreach (Player player in players.playerList)
                {
                    if (player.nationality == nation.name)
                    {
                        player.currentGoalsScored = 0;
                        player.status = QualifyingStatus.NonCandidate;
                        playerlist.Add(player);
                        //int offset = CalculateForm();
                        ////print("form: " + offset);
                        //player.goalScorePercentage = Mathf.Max(player.baseGoalScorePercentage + offset, 5);
                        //player.penaltyScorePercentage = Mathf.Min(player.basePenaltyScorePercentage + offset, 95);
                    }
                }
                if (nation.InferiorCountry)
                {
                    AssignRole(playerlist, QualifyingStatus.InferiorCandidate);
                    continue;
                }
                int qualifyer = nation.numQualifiers;
                while (qualifyer > 0)
                {
                    AssignRole(playerlist, QualifyingStatus.AutoQualifier);
                    qualifyer--;
                }
                int strongCandidate = nation.numStrongCandidate;
                while (strongCandidate > 0)
                {
                    AssignRole(playerlist, QualifyingStatus.StrongCandidate);
                    strongCandidate--;
                }
                int weakCandidate = nation.numWeakCandidate;
                while (weakCandidate > 0)
                {
                    AssignRole(playerlist, QualifyingStatus.WeakCandidate);
                    weakCandidate--;
                }
            }
            else
            {
                first.currentGoalsScored = 0;
                second.currentGoalsScored = 0;
                third.currentGoalsScored = 0;
                List<Player> firsts = new List<Player> { first };
                List<Player> seconds = new List<Player> { second };
                List<Player> thirds = new List<Player> { third };
                AssignRole(firsts, QualifyingStatus.AutoQualifier);
                AssignRole(seconds, QualifyingStatus.StrongCandidate);
                AssignRole(thirds, QualifyingStatus.WeakCandidate);
            }
        }       
        int ExtractInferiors = 4;
        while (ExtractInferiors > 0)
        {
            AssignRole(inferiorCandidates, QualifyingStatus.AutoQualifier);
            ExtractInferiors--;
        }
        ResetData();
        GetComponent<Qualifiers>().RunQualifiers();
    }

    private void ResetData()
    {
        inferiorCandidates.Clear();
    }

    public void AssignRole(List<Player> list, QualifyingStatus status)
    {
        //print("a");
        int sum = 0;
        foreach (Player player in list)
        {
            sum += (int)Mathf.Pow(player.goalScorePercentage, 2);
        }
        int index = Random.Range(0, sum + 1);
        //print("Index is " + index + " out of sum " + sum);
        foreach (Player player in list)
        {
            if (Mathf.Pow(player.goalScorePercentage, 2) >= index)
            {
                //print("b");
                player.status = status;
                list.Remove(player);
                if (status == QualifyingStatus.InferiorCandidate) inferiorCandidates.Add(player);
                else if (status == QualifyingStatus.WeakCandidate)
                {
                    GetComponent<Qualifiers>().weakCandidates.Add(player);
                }
                else if (status == QualifyingStatus.StrongCandidate)
                {
                    GetComponent<Qualifiers>().strongCandidates.Add(player);
                }
                break;
            }
            else index -= (int)Mathf.Pow(player.goalScorePercentage, 2);
        }
    }
}
