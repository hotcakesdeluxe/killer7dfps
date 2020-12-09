using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Health : ActorBehaviour
{
    [SerializeField]private float _maxHealth;
    public float health {get; private set;}
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
