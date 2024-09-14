using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {GS_GAME, GS_PAUSEMENU}

public class GameManager : MonoBehaviour
{
    public GameState currentGameState = GameState.GS_GAME;

    public static GameManager instance;
    public Canvas inGameCanvas;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Awake()
    {
        instance = this;
    }    

    private void SetGameState(GameState state)
    {
        currentGameState = state;

        if (currentGameState == GameState.GS_GAME)
            inGameCanvas.enabled = true;
        else
            inGameCanvas.enabled = false;
        
    }

    private void PauseMenu()
    {
        SetGameState(GameState.GS_PAUSEMENU);
    }

    private void InGame()
    {
        SetGameState(GameState.GS_GAME);
    }
}
