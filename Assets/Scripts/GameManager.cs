using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public enum GameState { GS_PAUSEMENU, GS_BATTLE, GS_LEVEL_COMPLETED, GS_GAME_LOST, GS_GAME_WON, GS_OPTIONS, GS_PREPARE }

public class GameManager : MonoBehaviour
{

    private TIleMapGenerator tilemap;
    public GameState currentGameState = GameState.GS_PREPARE;

    public static GameManager instance;
    public Canvas inGameCanvas;
    public GameObject rum;

    [SerializeField] private GameUIController inGameUI;
    public GameObject pauseUI;

    private const int levelsNum = 1;
    private int currentLevel = 0;
    private int currentWave = 0;
    private int[] waves = new int[levelsNum];
    private List<List<int>> enemiesHp = new List<List<int>>();      //to set the power of each wave in each level

    [SerializeField] private CardRollManager cardRollManager;


    void Start()
    {
        for(int i = 0; i <  levelsNum; i++)
        {
            Debug.Log("i" + i);
            enemiesHp.Add(new List<int>());
        }
        SetGameState(GameState.GS_PREPARE);
        waves[0] = 2;
        enemiesHp[0].Add(20);
        enemiesHp[0].Add(70);
        cardRollManager.setTotalHp(enemiesHp[0][0]);

    }


   
    public Vector2 getRumPosition()
    {
        return rum.transform.position;
    }
    public void setTilemap(TIleMapGenerator tilemap_)
    {
        this.tilemap = tilemap_;
    }

    public TIleMapGenerator getTilemap()
    {
        return this.tilemap;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Update");
        if (Input.GetKeyUp(KeyCode.P))
        {
        
            if (currentGameState == GameState.GS_PAUSEMENU)
            { 
                InGame();
            }
            else
            {
                PauseMenu();
            }
        }
    }

    public void Awake()
    {
        instance = this;
    }    

    private void SetGameState(GameState state)
    {
        currentGameState = state;
 
        pauseUI.SetActive(currentGameState == GameState.GS_PAUSEMENU);
       
        inGameUI.gameObject.SetActive(currentGameState == GameState.GS_BATTLE || currentGameState == GameState.GS_PREPARE);
        inGameCanvas.enabled = (currentGameState == GameState.GS_PREPARE || currentGameState == GameState.GS_PREPARE);
    }
   
    public void PauseMenu()
    {
        SetGameState(GameState.GS_PAUSEMENU);
    }

    public void InGame()
    {
        SetGameState(GameState.GS_BATTLE);
    }

    public void Prepare()
    {
        SetGameState(GameState.GS_PREPARE);
        inGameUI.GetComponent<GameUIController>().EnableReadyButton();
        
    }

    
    
    public void GameLost()
    {
        SceneManager.LoadScene("GameLost");
        SetGameState(GameState.GS_GAME_LOST);
    }

    public void GameWon()
    {
        SetGameState(GameState.GS_GAME_WON);
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayAgain()
    {
        SetGameState(GameState.GS_PREPARE);
        SceneManager.LoadScene("Scene");
    }

    private void NextLevel()
    {
        if(currentLevel == levelsNum - 1)
        {
            GameWon();
        }
        else
        {
            currentLevel++;
        }
    }

    private void NextWave()
    {
        currentWave++;
        Prepare();
        cardRollManager.setTotalHp(enemiesHp[currentLevel][currentWave]);
        cardRollManager.StartRolling();
        
    }
 
    public void WaveOver()
    {
        waves[currentLevel]--;
        if (waves[currentLevel] == 0)
        {
            NextLevel();
            inGameUI.UpdateRound(currentLevel, currentWave);
        }
        else
        {
            NextWave();
            inGameUI.UpdateRound(currentLevel, currentWave);
        }
    }
}
