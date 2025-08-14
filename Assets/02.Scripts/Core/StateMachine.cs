using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private IState currentState;

    public void SetState(IState state)
    {
        // ���ο� ���°� ���� ���¿� �����ϸ� ������ �� ����
        if(currentState.GetType().Name == state.GetType().Name)
        {
            currentState.ReEnterState();
            return;
        }

        // ���� ������ ���� ����
        currentState.ExitState();
        currentState = state;

        // ���ο� ������ ���� ����
        currentState.EnterState();
    }


}
