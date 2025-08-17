public class NPCReturnState : NPCState
{
    public NPCReturnState(NPC npc, NPCStateMachine stateMachine) : base(npc, stateMachine) { }

    public override void EnterState()
    {
        // 귀환 지점이 없으면 Idle롷 변경, 있으면 귀환 지점으로 이동
        if(npc.HomePoint == null) stateMachine.ChangeState<NPCIdleState>();
        else npc.MoveTo(npc.HomePoint.position);
    }

    public override void UpdateState()
    {
        if (npc.NearestEnemy != null)
        {
            if (npc.CanAttack)
                stateMachine.ChangeState<NPCAttackState>();
            else
                stateMachine.ChangeState<NPCFleeState>();
        }
        else if (npc.Agent.remainingDistance < 0.1f)
        {
            stateMachine.ChangeState<NPCIdleState>();
        }
    }
}
