using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class StateMachine : MonoBehaviour
{
    private IState currentState;
    private readonly Dictionary<Type, IState> states = new();
    [SerializeField] private string CurrentState;

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
        CurrentState = currentState.GetType().Name;
    }

    public void UpdateState()
    {
        currentState?.UpdateState();
    }

}
