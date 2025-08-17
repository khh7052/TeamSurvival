using UnityEngine;
public class NPCAttackState : NPCState
{
    public NPCAttackState(NPC npc, NPCStateMachine stateMachine) : base(npc, stateMachine) { }

    public override void EnterState() => npc.StopMoving();

    public override void UpdateState()
    {
        if (npc.NearestEnemy == null)
        {
            stateMachine.ChangeState<NPCReturnState>();
            return;
        }

        npc.transform.LookTarget(npc.NearestEnemy, npc.LookSpeed);

        if (npc.transform.IsTargetInDistance(npc.NearestEnemy, npc.AttackDistance))
        {
            npc.StopMoving();
            npc.TryAttack();
        }
        else
        {
            Vector3 desination = npc.NearestEnemy.position.ClampDistanceFrom(npc.HomePoint.position, npc.LimitMoveRange);
            npc.MoveTo(desination);
        }
    }
}
