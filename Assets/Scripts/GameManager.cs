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
    
    [SerializeField] private EnemyManager enemyManager;

    private const int levelsNum = 2;
    public int currentLevel = 0;
    public int currentWave = 0;
    private int[] waves = new int[levelsNum];
    private List<List<int>> enemiesHp = new List<List<int>>();      //to set the power of each wave in each level

    [SerializeField] private CardRollManager cardRollManager;
    private GameState prevState = GameState.GS_PREPARE;
    public GameObject[] rums;



    public ChainControler chainControler;

    public ShopManager shopManager;

    [SerializeField] private GameObject[] levelsLayout;

    void Start()
    {
        //
        ScoreManager.Instance.resetScore();
        
        levelsLayout[0].SetActive(true);
        Debug.Log("Start");
        for (int i = 0; i <  levelsNum; i++)
        {
            //Debug.Log("i" + i);
            enemiesHp.Add(new List<int>());
        }
        SetGameState(GameState.GS_WAIT);
        waves[0] = 3;
        waves[1] = 3;
        
        enemiesHp[0].Add(14);
        enemiesHp[0].Add(13); 
        enemiesHp[0].Add(22);
        
        enemiesHp[1].Add(20);
        enemiesHp[1].Add(24);
        enemiesHp[1].Add(30);

        cardRollManager.setTotalHp(enemiesHp[0][0]);
        cardRollManager.setBiggestEnemyValue(14); //Biggest enemy for first level is shark (11). It will increase by 1 in future waves.
        roundWonCanvas.enabled = false;

        if (enemyManager != null)
        {
            enemyManager.SetHealthAddition(5);
        }
        
        //NextLevel();
        PlayerPrefs.SetInt("Level", currentLevel+1); // Save current level to PlayerPrefs
        PlayerPrefs.Save();
    }


   
    public Vector2 getRumPosition()
    {
        return rums[currentLevel].transform.position;
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
        Debug.Log("Awake");
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
        Debug.Log($"Game State Changing: {currentGameState} -> {state}");

        // Handle visibility of CardColumn
        if (state == GameState.GS_BATTLE)
        {
            Debug.Log("Entering GS_BATTLE: Hiding column.");
            cardRollManager.ClearCards(); // This hides the column
            cardRollManager.cardColumn.gameObject.SetActive(false); // Explicitly hide it
        }
        else if (state == GameState.GS_PREPARE || state == GameState.GS_WAIT)
        {
            Debug.Log("Entering GS_PREPARE or GS_WAIT: Showing column.");
            cardRollManager.cardColumn.gameObject.SetActive(true); // Ensure column is visible
        }

        // Save the previous state if transitioning from non-pause and non-settings
        if (currentGameState != GameState.GS_PAUSEMENU && currentGameState != GameState.GS_SETTINGS)
        {
            prevState = currentGameState;
        }

        currentGameState = state;

        // Update UI based on the state
        pauseUI.SetActive(currentGameState == GameState.GS_PAUSEMENU);
        inGameUI.gameObject.SetActive(currentGameState == GameState.GS_BATTLE || currentGameState == GameState.GS_PREPARE || currentGameState == GameState.GS_WAIT);
        inGameCanvas.enabled = (currentGameState == GameState.GS_PREPARE || currentGameState == GameState.GS_WAIT);
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
            MarketManager.instance.earnGold(50); //Earn some gold for the next level.
            //chainControler.resetAnimations();
            Debug.Log(currentLevel + " now");
            levelsLayout[currentLevel].SetActive(false);
            ++currentLevel;
            levelsLayout[currentLevel].SetActive(true);
            chainControler.RemoveAll(); //TODO: Also needs to remove un-chained items on the tiles!
            EnemyPathManager.Instance.NextLevel();
            Debug.Log(currentLevel + " now2");
            currentWave = 0;
            Wait();
            inGameUI.UpdateRound(currentLevel, currentWave);
            PlayerPrefs.SetInt("Level", currentLevel+1); // Save current level to PlayerPrefs
            PlayerPrefs.Save();
            cardRollManager.setTotalHp(enemiesHp[currentLevel][currentWave]);
            cardRollManager.setBiggestEnemyValue(cardRollManager.getBiggestEnemyValue()+1); //Increase biggest enemy to be faced.
            cardRollManager.StartRolling();
            inGameUI.UpdateGoldAmount(MarketManager.instance.Gold);
            //Increase bonus health by 5
            enemyManager.SetHealthAddition(enemyManager.getHealthAddition()+5);
        }
        
        
    }

    private void NextWave()
    {
        //chainControler.resetAnimations();
        currentWave++;
        inGameUI.UpdateRound(currentLevel, currentWave);
        cardRollManager.setTotalHp(enemiesHp[currentLevel][currentWave]);
        cardRollManager.setBiggestEnemyValue(cardRollManager.getBiggestEnemyValue()+1); //Increase biggest possible enemy to be faced.
        cardRollManager.StartRolling();
        
        PlayerPrefs.SetInt("Wave", currentWave+1); // Save current wave to PlayerPrefs
        PlayerPrefs.Save();
        
    }
 
    public IEnumerator WaveOver()
    {
        
        Wait();
        chainControler.resetAnimations();
        chainControler.StopRolling();
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
