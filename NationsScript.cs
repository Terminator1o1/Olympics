using RPG.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NationsScript : MonoBehaviour, ISaveable
{
    [SerializeField] Nations nations;
    [SerializeField] public Nation[] nationList;
    [SerializeField] public NationIntArray[] nationIntArrays;
 
    [System.Serializable]
    public class NationIntArray
    {
        [SerializeField] public int[] stats;
    }



    public object CaptureState()
    {
        nationList = nations.nationList;
        NationIntArray[] newNationsIntArrays = new NationIntArray[nationList.Length];
        for (int i = 0; i < nationList.Length; i++)
        {
            newNationsIntArrays[i] = new NationIntArray();
            newNationsIntArrays[i].stats = new int[4];
            newNationsIntArrays[i].stats[0] = nationList[i].worldCupTableSortingPoints;
            newNationsIntArrays[i].stats[1] = nationList[i].worldCupFirst;
            newNationsIntArrays[i].stats[2] = nationList[i].worldCupSecond;
            newNationsIntArrays[i].stats[3] = nationList[i].worldCupThird;
        }
        return newNationsIntArrays;
    }

    public void RestoreState(object state)
    {
        nationList = nations.nationList;
        NationIntArray[] newNationsIntArrays = (NationIntArray[])state;
        for (int i = 0; i < nationList.Length; i++)
        {
            nationList[i].worldCupTableSortingPoints = newNationsIntArrays[i].stats[0];
            nationList[i].worldCupFirst = newNationsIntArrays[i].stats[1];
            nationList[i].worldCupSecond = newNationsIntArrays[i].stats[2];
            nationList[i].worldCupThird= newNationsIntArrays[i].stats[3];
        }
        nations.nationList = nationList;
    }
}
