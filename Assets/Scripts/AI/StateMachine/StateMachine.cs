using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    IState<T> currentState;

    public void ChangeState(IState<T> newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    public string GetState()
    {
        return currentState.ToString();
    }

    public void Update()
    {
        if (currentState != null) currentState.Execute();
    }

    public void FixedUpdate()
    {
        if (currentState != null) currentState.ExecuteFixed();
    }
}
