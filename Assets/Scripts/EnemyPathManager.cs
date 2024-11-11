using System;
using System.Collections.Generic;
using UnityEngine;


class EnemyPathManager : MonoBehaviour
{
    public static EnemyPathManager Instance { get; private set; }

    [SerializeField] GameObject[] allPaths;
    
    private void Start()
    {
        Instance = this;
        
    }

    public GameObject[] getRandomPath()
    {
        //Debug.Log("Method called");
        int pathsNum = allPaths.Length;
        System.Random rand = new System.Random();
        int idx = rand.Next(0, pathsNum);

        int childrenNum = allPaths[idx].transform.childCount;
        GameObject[] chosenPath = new GameObject[childrenNum];
        for (int i = 0; i < childrenNum; i++)
        {
            chosenPath[i] = allPaths[idx].transform.GetChild(i).gameObject;
        }
        //Debug.Log("Idx: " + idx + " Length " + chosenPath.Length);
        return chosenPath;
    }


}