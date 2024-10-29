using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public enum GameState { GS_PAUSEMENU, GS_GAME, GS_LEVELCOMPLETED, GS_GAME_OVER, GS_OPTIONS, GS_PREPARE }

public class GameManager : MonoBehaviour
{

    private TIleMapGenerator tilemap;
    public GameState currentGameState = GameState.GS_PREPARE;

    public static GameManager instance;
    public Canvas inGameCanvas;
    public GameObject rum;

    public GameObject inGameUI;
    public GameObject pauseUI;
    void Start()
    {
        SetGameState(GameState.GS_PREPARE);
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
        inGameUI.SetActive(currentGameState == GameState.GS_GAME || currentGameState == GameState.GS_PREPARE);
        inGameCanvas.enabled = (currentGameState == GameState.GS_GAME || currentGameState == GameState.GS_PREPARE);
    }
   
    private void PauseMenu()
    {
        SetGameState(GameState.GS_PAUSEMENU);
    }

    public void InGame()
    {
        SetGameState(GameState.GS_GAME);
    }

    
    
    public void GameLost()
    {
        SceneManager.LoadScene("GameLost");
        SetGameState(GameState.GS_GAME_OVER);
    }

    public void PlayAgain()
    {
        SetGameState(GameState.GS_GAME);
        SceneManager.LoadScene("Scene");
    }
}
