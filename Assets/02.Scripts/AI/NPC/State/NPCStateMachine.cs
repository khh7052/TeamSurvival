using UnityEngine;

public class NPCStateMachine : StateMachine
{
    protected NPC npc;

    public void Initialize(NPC npc)
    {
        this.npc = npc;

        // 상태 추가
        AddState(new NPCIdleState(npc, this));
        AddState(new NPCFleeState(npc, this));
        AddState(new NPCAttackState(npc, this));
        AddState(new NPCReturnState(npc, this));

        ChangeState<NPCIdleState>();
    }
}
