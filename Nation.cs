using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Nation
{
    [SerializeField] public string name;
    [SerializeField] public int numQualifiers;
    [SerializeField] public int numStrongCandidate;
    [SerializeField] public int numWeakCandidate;
    [SerializeField] public bool InferiorCountry;
    [SerializeField] public Texture flag;
    [SerializeField] public AudioClip anthem;
    [SerializeField] public int gold = 0;
    [SerializeField] public int silver = 0;
    [SerializeField] public int bronze = 0;
    [SerializeField] public int medalPoints = 0;
    [SerializeField] public int rating = 0;
    [SerializeField] public int worldCupPoints = 0;
    [SerializeField] public int worldCupGroupStagesWins = 0;
    [SerializeField] public int worldCupGroupStagesGoals = 0;
    [SerializeField] public int tier = 0;
    [SerializeField] public int worldCupSortingPoints = 0;
    [SerializeField] public int worldCupFirst = 0;
    [SerializeField] public int worldCupSecond = 0;
    [SerializeField] public int worldCupThird = 0;
    [SerializeField] public int worldCupTableSortingPoints = 0;
}
