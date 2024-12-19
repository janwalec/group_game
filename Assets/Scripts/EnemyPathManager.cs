using System;
using System.Collections.Generic;
using UnityEngine;


class EnemyPathManager : MonoBehaviour
{
    public static EnemyPathManager Instance { get; private set; }

    [SerializeField] GameObject[] allPaths;
    [SerializeField] GameObject[] allPathsLv2;
    [SerializeField] GameObject[] allPathsLv3;

    [SerializeField] GameObject[] allPathsLv4;
    [SerializeField] GameObject[] allPathsLv5;
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

        int pathsNum = 0;

        switch(currentPaths){
            case 1:
                pathsNum = allPaths.Length;
                break;
            case 2:
                pathsNum = allPathsLv2.Length;
                break;
            case 3:
                pathsNum = allPathsLv3.Length;
                break;
            
            case 4:
                pathsNum = allPathsLv4.Length;
                break;
            case 5:
                pathsNum = allPathsLv5.Length;
                break;
            
        }
            
        System.Random rand = new System.Random();
        int idx = rand.Next(0, pathsNum);

        //int childrenNum = currentPaths == 1 ? allPaths[idx].transform.childCount: allPathsLv2[idx].transform.childCount;
        int childrenNum = 0;

        switch(currentPaths){
            case 1:
                childrenNum = allPaths[idx].transform.childCount;
                break;
            case 2:
                childrenNum = allPathsLv2[idx].transform.childCount;
                break;
            case 3:
                childrenNum = allPathsLv3[idx].transform.childCount;
                break;
            case 4:
                childrenNum = allPathsLv4[idx].transform.childCount;
                break;
            case 5:
                childrenNum = allPathsLv5[idx].transform.childCount;
                break;
            
        }

        
        GameObject[] chosenPath = new GameObject[childrenNum];
        for (int i = 0; i < childrenNum; i++)
        {

            switch(currentPaths){
            case 1:
                chosenPath[i] = allPaths[idx].transform.GetChild(i).gameObject;
                break;
            case 2:
                chosenPath[i] = allPathsLv2[idx].transform.GetChild(i).gameObject;
                break;
            case 3:
                 chosenPath[i] = allPathsLv3[idx].transform.GetChild(i).gameObject;
                break;
            case 4:
                 chosenPath[i] = allPathsLv4[idx].transform.GetChild(i).gameObject;
                break;
            case 5:
                 chosenPath[i] = allPathsLv5[idx].transform.GetChild(i).gameObject;
                break;
            
        
            }
        }
        //Debug.Log("Idx: " + idx + " Length " + chosenPath.Length);
        return chosenPath;
    }


}