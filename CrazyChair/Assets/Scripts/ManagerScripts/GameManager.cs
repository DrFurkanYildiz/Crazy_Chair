using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;
    public GameState State { get; private set; }

    void Awake() => Instance = this;
    void Start() => ChangeState(GameState.Starting);
    public void ChangeState(GameState newState)
    {
        OnBeforeStateChanged?.Invoke(newState);

        State = newState;

        switch (newState)
        {
            case GameState.Starting:
                HandleStarting();
                break;
            case GameState.SpawningGuest:
                HandleSpawningGuest();
                break;
            case GameState.Continue:
                ContinueGame();
                break;
            case GameState.Pause:
                PauseGame();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        OnAfterStateChanged?.Invoke(newState);

    }

    private void HandleStarting()
    {
        if (SaveLoadSystem.isHaveLoadFile("SystemSave"))
            SaveManager.Instance.OnLoad();

        ChangeState(GameState.SpawningGuest);
    }
    private void HandleSpawningGuest()
    {
        HumanSpawner.Instance.Setup();
        ChairManager.Instance.Setup();
        QueueManager.Instance.Setup();
        GameData.Instance.Setup();
        UISystem.Instance.Setup();
        StoreController.Instance.Setup();

        ChangeState(GameState.Continue);
    }
    private void ContinueGame()
    {
        Time.timeScale = 1;
    }
    private void PauseGame()
    {
        Time.timeScale = 0;
    }


    public void PauseGameButton() => ChangeState(GameState.Pause);
    public void ContinueGameButton() => ChangeState(GameState.Continue);
}

[Serializable]
public enum GameState
{
    Starting,
    SpawningGuest,
    Continue,
    Pause,
    SpeedUp,
    Restart,
    MainMenu,
    Win,
    Lose,
}