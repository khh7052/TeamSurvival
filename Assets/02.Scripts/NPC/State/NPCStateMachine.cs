using UnityEngine;

public class NPCStateMachine : StateMachine
{
    public NPCIdleState IdleState { get; private set; }
    public NPCFleeState FleeState { get; private set; }
    public NPCAttackState AttackState { get; private set; }
    public NPCReturnState ReturnState { get; private set; }

    protected NPC npc;

    public void Initialize(NPC npc)
    {
        this.npc = npc;

        // 상태 객체 한 번만 생성
        IdleState = new(npc, this);
        FleeState = new(npc, this);
        AttackState = new(npc, this);
        ReturnState = new(npc, this);

        SetState(IdleState); // 시작 상태

    }
}
