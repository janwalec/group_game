using System;
using System.Collections.Generic;
using UnityEngine;


class EnemyPathManager : MonoBehaviour
{
    public static EnemyPathManager Instance { get; private set; }

    [SerializeField] GameObject[] allPaths;
    [SerializeField] GameObject[] allPathsLv2;
    private int currentPaths = 1;
    private void Awake()
    {
        Instance = this;
        
    }

    public void NextLevel()
    {
        currentPaths++;
    }
    public GameObject[] getRandomPath()
    {

        //Debug.Log("Method called");

        int pathsNum = currentPaths == 1 ? allPaths.Length: allPathsLv2.Length;
        System.Random rand = new System.Random();
        int idx = rand.Next(0, pathsNum);

        int childrenNum = currentPaths == 1 ? allPaths[idx].transform.childCount: allPathsLv2[idx].transform.childCount;
        GameObject[] chosenPath = new GameObject[childrenNum];
        for (int i = 0; i < childrenNum; i++)
        {
            chosenPath[i] = currentPaths == 1 ? allPaths[idx].transform.GetChild(i).gameObject : allPathsLv2[idx].transform.GetChild(i).gameObject;
        }
        //Debug.Log("Idx: " + idx + " Length " + chosenPath.Length);
        return chosenPath;
    }


}