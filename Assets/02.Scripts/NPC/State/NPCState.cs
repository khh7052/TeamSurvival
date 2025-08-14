public class NPCState : IState
{
    protected NPC npc;
    protected NPCStateMachine stateMachine;
    protected NPCState(NPC npc, NPCStateMachine stateMachine)
    {
        this.npc = npc;
        this.stateMachine = stateMachine;
    }

    public virtual void EnterState() { }

    public virtual void UpdateState() { }

    public virtual void ExitState() { }

    public virtual void ReEnterState() { }
}
