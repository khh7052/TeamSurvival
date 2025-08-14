using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private IState currentState;

    public void SetState(IState state)
    {
        // 현재 상태가 null이거나 새로 설정할 상태가 null인 경우
        if(currentState == null || state == null) 
        {
            currentState = state;
            currentState?.EnterState();
            return;
        }

        // 새로운 상태가 기존 상태와 동일하면 재진입 후 리턴
        if (currentState.GetType().Name == state.GetType().Name)
        {
            currentState.ReEnterState();
            return;
        }

        // 현재 상태의 종료 실행
        currentState.ExitState();
        currentState = state;

        // 새로운 상태의 진입 실행
        currentState.EnterState();
    }

    public void UpdateState()
    {
        currentState?.UpdateState();
    }

}
