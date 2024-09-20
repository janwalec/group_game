using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum GameState { GS_PAUSEMENU, GS_GAME, GS_LEVELCOMPLETED, GS_GAME_OVER, GS_OPTIONS }

public class GameManager : MonoBehaviour
{

    private TIleMapGenerator tilemap;
    public GameState currentGameState = GameState.GS_GAME;

    public static GameManager instance;
    public Canvas inGameCanvas;

    void Start()
    {
        
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
