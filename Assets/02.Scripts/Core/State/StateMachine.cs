using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private IState currentState;
    private readonly Dictionary<Type, IState> states = new();

    public void AddState(IState state) 
        => states[state.GetType()] = state;

    public void ChangeState<T>() where T : IState
    {
        // 현재 상태가 null인 경우
        if (currentState == null)
        {
            currentState = states[typeof(T)];
            currentState?.EnterState();
            return;
        }

        // 새로운 상태가 기존 상태와 동일하면 재진입 후 리턴
        if (currentState?.GetType() == typeof(T))
        {
            currentState.ReEnterState();
            return;
        }

        // 현재 상태의 종료
        currentState?.ExitState();

        // 새로운 상태의 진입
        currentState = states[typeof(T)];
        currentState.EnterState();
    }

    public void UpdateState()
    {
        currentState?.UpdateState();
    }

}
