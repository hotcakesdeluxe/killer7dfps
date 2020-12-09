using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_EnemyAnimation : ActorBehaviour
{
    [SerializeField]private Animator _animator;
    public Animator animator => _animator;

    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);

    }
    public override void InitializeBehaviour(Actor newActor)
    {
        base.InitializeBehaviour(newActor);
    }
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
    }
}
