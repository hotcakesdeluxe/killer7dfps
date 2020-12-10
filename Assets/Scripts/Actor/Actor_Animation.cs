using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Animation : ActorBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Animator _FPSanimator;
    public Animator animator => _animator;
    private Actor_CharacterController _characterController;
    private Actor_PlayerInput _input;
    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);
        _characterController = GetBehaviour<Actor_CharacterController>();
        _input = GetBehaviour<Actor_PlayerInput>();
    }
    public override void InitializeBehaviour(Actor newActor)
    {
        base.InitializeBehaviour(newActor);
    }
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
        _animator.SetFloat("MoveSpeed", Mathf.Abs(_characterController.forwardAmount) * 10);
        _animator.SetBool("IsAiming", _input.isAiming);

        if (_input.isAiming)
        {
            _FPSanimator.SetBool("IsAiming", _input.isAiming);
            _FPSanimator.SetBool("IsFiring", _input.isFiring);
        }
        
    }
    public void PlayReload()
    {
        _animator.Play("Reload");
    }
}
