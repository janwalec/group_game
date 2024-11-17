using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public enum GameState { GS_PAUSEMENU, GS_BATTLE, GS_LEVEL_COMPLETED, GS_GAME_LOST, GS_GAME_WON, GS_SETTINGS, GS_PREPARE, GS_WAIT }

public class GameManager : MonoBehaviour
{

    private TIleMapGenerator tilemap;
    public GameState currentGameState = GameState.GS_PREPARE;

    public static GameManager instance;

    public Canvas inGameCanvas;
    public Canvas roundWonCanvas;
    [SerializeField] private GameUIController inGameUI;
    [SerializeField] private GameObject levelWonUI;
    public GameObject pauseUI;
    public GameObject settingsUI;

    private const int levelsNum = 2;
    private int currentLevel = 0;
    private int currentWave = 0;
    private int[] waves = new int[levelsNum];
    private List<List<int>> enemiesHp = new List<List<int>>();      //to set the power of each wave in each level

    [SerializeField] private CardRollManager cardRollManager;
    private GameState prevState = GameState.GS_PREPARE;
    public GameObject rum;

    public ChainControler chainControler;

    public ShopManager shopManager;

    void Start()
    {
        for(int i = 0; i <  levelsNum; i++)
        {
            //Debug.Log("i" + i);
            enemiesHp.Add(new List<int>());
        }
        SetGameState(GameState.GS_WAIT);
        waves[0] = 2;
        waves[1] = 2;
        enemiesHp[0].Add(10);
        enemiesHp[0].Add(10);
       // enemiesHp[0].Add(10);
        enemiesHp[1].Add(20);
        enemiesHp[1].Add(20);

        cardRollManager.setTotalHp(enemiesHp[0][0]);
        roundWonCanvas.enabled = false;

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

    public void setShopManager(ShopManager shopManager_) {
        this.shopManager = shopManager_;
    }

    public ShopManager getShopManager() {
        return this.shopManager;
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

    public void StartRolling()
    {
        chainControler.rollAllModifiers();
    }
    public void StartGame()
    {
        //chainControler.rollAllModifiers();
        SetGameState(GameState.GS_BATTLE);

        // Find the CaptainShip instance and start its spawn routine
        CaptainShip captainShip = FindObjectOfType<CaptainShip>();
        if (captainShip != null)
        {
            captainShip.StartSpawningEnemies();
        }
        else
        {
            Debug.LogError("CaptainShip not found in the scene.");
        }
    }

    private void SetGameState(GameState state)
    {
        /*if(prevState == GameState.GS_PREPARE && state != GameState.GS_PREPARE)
        {
            inGameUI.DisableReadyButton();
        }*/

        // Clear cards when entering BATTLE state
        if (currentGameState == GameState.GS_BATTLE)
        {
            cardRollManager.ClearCards();
        }

        if (currentGameState != GameState.GS_PAUSEMENU && currentGameState != GameState.GS_SETTINGS)
            prevState = currentGameState;
        
        currentGameState = state;
        //Debug.Log(currentGameState);
        pauseUI.SetActive(currentGameState == GameState.GS_PAUSEMENU);
        inGameUI.gameObject.SetActive(currentGameState == GameState.GS_BATTLE || currentGameState == GameState.GS_PREPARE || currentGameState == GameState.GS_WAIT);
        inGameCanvas.enabled = (currentGameState == GameState.GS_PREPARE || currentGameState == GameState.GS_PREPARE);
        inGameUI.SetReadyButtonActive(currentGameState == GameState.GS_PREPARE);
        levelWonUI.SetActive(currentGameState == GameState.GS_LEVEL_COMPLETED);
        settingsUI.SetActive(currentGameState == GameState.GS_SETTINGS);
    }

    public void SetPreviousState()
    {
        SetGameState(prevState);
    }
   
    public void PauseMenu()
    {
        SetGameState(GameState.GS_PAUSEMENU);
    }

    public void InGame()
    {
        SetGameState(GameState.GS_BATTLE);
    }

    public void Wait()
    {
        SetGameState(GameState.GS_WAIT);
    }

    public void Settings()
    {
        SetGameState(GameState.GS_SETTINGS);
    }

    public void Prepare()
    {
        SetGameState(GameState.GS_PREPARE);
        //inGameUI.GetComponent<GameUIController>().EnableReadyButton();
        
    }

    public void LevelCompleted()
    {
        if (currentLevel < levelsNum - 1)
            SetGameState(GameState.GS_LEVEL_COMPLETED);
        else
            GameWon();
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

    //TODO

    public void NextLevel()
    {   
        Debug.Log("Call wait");
        if(currentLevel == levelsNum - 1)
        {
            GameWon();
        }
        else
        {
            //chainControler.resetAnimations();
            Debug.Log(currentLevel + " now");
            ++currentLevel;
            Debug.Log(currentLevel + " now2");
            currentWave = 0;
            Wait();
            inGameUI.UpdateRound(currentLevel, currentWave);
            cardRollManager.setTotalHp(enemiesHp[currentLevel][currentWave]);
            cardRollManager.StartRolling();
            
        }
        
    }

    private void NextWave()
    {
        //chainControler.resetAnimations();
        currentWave++;
        inGameUI.UpdateRound(currentLevel, currentWave);
        cardRollManager.setTotalHp(enemiesHp[currentLevel][currentWave]);
        cardRollManager.StartRolling();
        
    }
 
    public IEnumerator WaveOver()
    {

        Wait();
        Debug.Log("here2");
        if (waves[currentLevel] == currentWave + 1)
        {
            Debug.Log("Not display");
            Debug.Log(currentWave);
            LevelCompleted();
         //   inGameUI.UpdateRound(currentLevel, currentWave);
        }
        else
        {
            //display "Round won!" info
            Debug.Log("Try to display");
            roundWonCanvas.enabled = true;
            yield return new WaitForSeconds(1.5f);
            roundWonCanvas.enabled = false;


            NextWave();
           // inGameUI.UpdateRound(currentLevel, currentWave);

        }
        
    }

    /*
    private IEnumerator DisplayWaveWon()
    {
        
        roundWonCanvas.enabled = true;
        yield return new WaitForSeconds(1.5f);
        roundWonCanvas.enabled = false;
        
    }*/
}
