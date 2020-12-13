using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_HitReceiver : ActorBehaviour
{
    [SerializeField] private HitboxRoot _hitbox;

    private Actor_Health _health;

    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);

        _hitbox.hitEvent.AddListener(HitReceived);

        _health = GetBehaviour<Actor_Health>();
    }

    private void HitReceived(DamageInfo damageInfo)
    {
        if(!_health.isDead){
            if (damageInfo.damageType == DamageType.Base)
            {
                _health.LoseHealth(damageInfo.amount);
            }
            if (damageInfo.damageType == DamageType.Weakpoint)
            {
                _health.instaKilledEvent.Invoke();
            }
        }
    }
}
