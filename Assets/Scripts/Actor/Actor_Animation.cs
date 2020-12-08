using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Animation : ActorBehaviour
{
    [SerializeField]private Animator _animator;
    private Actor_CharacterController _characterController;
    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);
        _characterController = GetBehaviour<Actor_CharacterController>();
    }
    public override void InitializeBehaviour(Actor newActor)
    {
        base.InitializeBehaviour(newActor);
    }
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
        _animator.SetFloat("MoveSpeed", Mathf.Abs(_characterController.forwardAmount)*10 );
    }
}
