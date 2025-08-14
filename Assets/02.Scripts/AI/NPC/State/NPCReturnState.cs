public class NPCReturnState : NPCState
{
    public NPCReturnState(NPC npc, NPCStateMachine stateMachine) : base(npc, stateMachine) { }

    public override void EnterState() => npc.MoveTo(npc.HomePoint.position);

    public override void UpdateState()
    {
        if (npc.NearestEnemy != null)
        {
            if (npc.CanAttack)
                stateMachine.SetState(stateMachine.AttackState);
            else
                stateMachine.SetState(stateMachine.FleeState);
        }
        else if (npc.Agent.remainingDistance < 0.1f)
        {
            stateMachine.SetState(stateMachine.IdleState);
        }
    }
}
