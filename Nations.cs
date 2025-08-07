using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nations", menuName = "Nations/New Nations", order = 0)]
public class Nations : ScriptableObject
{
    [SerializeField] public Nation[] nationList = null;
    [SerializeField] public string[] worldCupContexts;

    public Nation GetNation(string name)
    {
        foreach (Nation nation in nationList)
        {
            if (nation.name == name) return nation;
        }
        return null;
    }

    public Nation[] GetNations(int val)
    {
        List<Nation> nationsList = new List<Nation>();
        foreach (Nation nation in nationList)
        {
            if (nation.medalPoints == val) nationsList.Add(nation);
        }
        return nationsList.ToArray();
    }

    public Nation[] GetNationsSortingPoints(int v)
    {
        List<Nation> resultNations = new List<Nation>();
        Nation[] nonInferior = GetNonInferiors();
        foreach (Nation nation in nonInferior)
        {
            if (nation.worldCupTableSortingPoints == v)
            {
                resultNations.Add(nation);
            }
        }
        return resultNations.ToArray();
    }

    public Nation[] GetNationsRatings(int v)
    {
        List<Nation> resultNations = new List<Nation>();
        Nation[] nonInferior = GetNonInferiors();
        foreach (Nation nation in nonInferior)
        {
            if (nation.rating == v)
            {
                resultNations.Add(nation);
            }
        }
        return resultNations.ToArray();
    }

    public Nation[] GetNonInferiors()
    {
        Nation[] results = new Nation[48];
        int i = 0;
        foreach (Nation nation in nationList)
        {
            if (!nation.InferiorCountry)
            {
                results[i] = nation;
                i++;
            }
        }
        return results;
    }
}
