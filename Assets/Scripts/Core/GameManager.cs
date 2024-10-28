using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;
using Data;

public class GameManager : Singleton<GameManager>
{
    public UserData UserData
    {
        get; private set;
    }
    protected override void Awake()
    {
        base.Awake();
        Game.Launch();
        UserData = Game.Data.Load<UserData>();
        
    }
   

    private void Start()
    {
        GameUI.Instance.Get<UIStart>().Show();
        DayManager.Instance.OnStart();
    }

   
    [SerializeField] private GameStates _state = GameStates.Pause;
    public void ChangeState(GameStates newState)
    {
        if (newState == _state) return;
        ExitCurrentState();
        _state = newState;
        EnterNewState();
    }

    public bool CheckGameState(GameStates newState)
    {
        if (_state == newState)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void EnterNewState()
    {

        switch (_state)
        {
            case GameStates.Tutorial:
                break;
            case GameStates.Home:
                break;
            case GameStates.Start:
                DayManager.Instance.LoadDay();
                break;
            case GameStates.Play:
              
                break;
            case GameStates.Pause:
                break;
            case GameStates.Win:

                break;
            case GameStates.Lose:

                break;
            default:
                break;
        }
    }

    private void ExitCurrentState()
    {
        switch (_state)
        {
            case GameStates.Tutorial:
                break;
            case GameStates.Home:
                break;
            case GameStates.Start:
                break;
            case GameStates.Play:
                //GameUI.Instance.Get<UITask>().TaskUpdate();
                break;
            case GameStates.Pause:
                break;
            case GameStates.Win:
                break;
            case GameStates.Lose:
                break;
            default:
                break;
        }
    }


    
}

public enum GameStates
{
    Play, Win, Lose, Home, Tutorial, Start, Pause
}
