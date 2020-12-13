using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_EnemyAnimation : ActorBehaviour
{
    private Actor_Enemy _enemy;
    private Actor_Navigation _navigation;
    private Actor_Health _health;
    [SerializeField]private Animator _animator;
    [SerializeField] private HitboxRoot _hitbox;
    public Animator animator => _animator;

    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);
        _enemy = GetBehaviour<Actor_Enemy>();
        _navigation = GetBehaviour<Actor_Navigation>();
        _health = GetBehaviour<Actor_Health>();
        _hitbox.hitEvent.AddListener(GetHit);

    }
    public override void InitializeBehaviour(Actor newActor)
    {
        base.InitializeBehaviour(newActor);
        _navigation.attackRangeEvent.AddListener(PlayAttack);
    }
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
        _animator.SetFloat("Velocity", _navigation.agent.velocity.magnitude);
        
    }
    private void GetHit(DamageInfo damageInfo)
    {
        _animator.Play("HitReact");
    }
    public void PlayDead()
    {
        _animator.CrossFade("Death", 0.2f);
    }
    private void PlayAttack()
    {
        _animator.Play("Attack");
    }
}
