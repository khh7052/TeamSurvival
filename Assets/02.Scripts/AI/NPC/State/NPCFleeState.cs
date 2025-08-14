using UnityEngine;

public class NPCFleeState : NPCState
{
    public NPCFleeState(NPC npc, NPCStateMachine stateMachine) : base(npc, stateMachine) { }

    public override void EnterState() => npc.MoveTo(npc.GetFleeLocation());

    public override void UpdateState()
    {
        if (npc.NearestEnemy != null)
        {
            npc.MoveTo(npc.GetFleeLocation());
        }
        else if (npc.Agent.remainingDistance < 0.1f)
        {
            stateMachine.SetState(stateMachine.ReturnState);
        }
    }
}
