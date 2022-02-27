using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public enum PlayerStatus {Sliding, Flying, Crashed, ReachedPool}
    public static PlayerStatus CurrentPlayerStatus;

    private InGameUI _inGameUi;
    
    void Start()
    {
        _inGameUi = FindObjectOfType<InGameUI>();
        CurrentPlayerStatus = PlayerStatus.Sliding;
    }

    public void EndGame()
    {
        switch (CurrentPlayerStatus)
        {
            case PlayerStatus.Crashed:
                _inGameUi.End(false);
                break; 
            case PlayerStatus.ReachedPool:
                _inGameUi.End(true);
                break;
            default:
                _inGameUi.End(false);
                break;
        }
    }
    
}
