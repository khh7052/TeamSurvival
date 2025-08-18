using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPatrolState : NPCState
{
    public NPCPatrolState(NPC npc, NPCStateMachine stateMachine) : base(npc, stateMachine) { }

    public override void EnterState()
    {
        base.EnterState();
        npc.TryPatrol();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (npc.NearestEnemy != null)
        {
            if (npc.CanAttack)
                stateMachine.ChangeState<NPCAttackState>();
            else
                stateMachine.ChangeState<NPCFleeState>();
            return;
        }
        else
        {
            if(npc.RemainingDistance < 0.1f)
            {
                npc.StopMoving();
                npc.TryPatrol();
            }
        }
    }

}
