using UnityEngine;

public class NPCIdleState : NPCState
{
    public NPCIdleState(NPC npc, NPCStateMachine stateMachine) : base(npc, stateMachine) { }

    public override void EnterState() => npc.StopMoving();

    public override void UpdateState()
    {
        if (npc.NearestEnemy != null)
        {
            if (npc.CanAttack)
                stateMachine.ChangeState<NPCAttackState>();
            else
                stateMachine.ChangeState<NPCFleeState>();
            return;
        }

        if (npc.PlayerDistance <= npc.DetectDistance)
            npc.transform.LookTarget(npc.Target, npc.LookSpeed);
    }
}
