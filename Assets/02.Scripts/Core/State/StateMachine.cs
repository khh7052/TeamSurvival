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
        // ���� ���°� null�� ���
        if (currentState == null)
        {
            currentState = states[typeof(T)];
            currentState?.EnterState();
            return;
        }

        // ���ο� ���°� ���� ���¿� �����ϸ� ������ �� ����
        if (currentState?.GetType() == typeof(T))
        {
            currentState.ReEnterState();
            return;
        }

        // ���� ������ ����
        currentState?.ExitState();

        // ���ο� ������ ����
        currentState = states[typeof(T)];
        currentState.EnterState();
    }

    public void UpdateState()
    {
        currentState?.UpdateState();
    }

}
