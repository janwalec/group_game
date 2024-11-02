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

    public GameObject inGameUI;
    public GameObject pauseUI;

    private const int levelsNum = 1;
    private int currentLevel = 0;
    private int[] waves = new int[levelsNum];

    [SerializeField] private CardRollManager cardRollManager;


    void Start()
    {
        SetGameState(GameState.GS_PREPARE);
        waves[0] = 2;
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
        inGameUI.SetActive(currentGameState == GameState.GS_BATTLE || currentGameState == GameState.GS_PREPARE);
        inGameCanvas.enabled = (currentGameState == GameState.GS_PREPARE || currentGameState == GameState.GS_PREPARE);
    }
   
    private void PauseMenu()
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
        Prepare();
        cardRollManager.StartRolling();
        
    }
 
    public void WaveOver()
    {
        waves[currentLevel]--;
        if (waves[currentLevel] == 0)
        {
            NextLevel();
        }
        else
        {
            NextWave();
        }
    }
}
