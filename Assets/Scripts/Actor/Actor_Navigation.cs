using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using PHL.Common.Utility;

public class Actor_Navigation : ActorBehaviour
{
    private Actor_Health _health;
    [SerializeField] private NavMeshAgent _agent;
    public NavMeshAgent agent => _agent;
    [SerializeField] private SphereCollider _detectionRadius;
    [SerializeField] private HitboxRoot _hitbox;
    private Actor _player;
    public LayerMask playerLayer;
    private bool _inAttackRange = false;
    public SecureEvent attackRangeEvent {get; private set;} = new SecureEvent();
    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);
        _health = GetBehaviour<Actor_Health>();
    }
    public override void InitializeBehaviour(Actor newActor)
    {
        base.InitializeBehaviour(newActor);
        _hitbox.hitEvent.AddListener(HitStun);
    }
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
        if (!_health.isDead)
        {
            Detection();
            if (_player != null && _agent.isActiveAndEnabled)
            {
                _agent.SetDestination(_player.gameObject.transform.position);
                if (_agent.remainingDistance <= 1.4f && !_inAttackRange)
                {
                    Debug.Log("tryna blow up and act like i don't know nobody");
                    attackRangeEvent.Invoke();
                    _inAttackRange = true;
                }
            }

        }
        else
        {
            _agent.SetDestination(transform.position);
        }
    }
    public void Detection()
    {
        Collider[] overlaps = _detectionRadius.OverlapSphere(playerLayer);
        foreach (Collider overlap in overlaps)
        {
            _player = overlap.GetComponentInParent<Actor>();
        }
    }
    public void HitStun(DamageInfo damageInfo)
    {
        StartCoroutine(HitStunRoutine());
    }
    private IEnumerator HitStunRoutine()
    {
        _agent.speed = 0f;
        yield return new WaitForSeconds(1);
        _agent.speed = 1f;
    }
}
