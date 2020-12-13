using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PHL.Common.Utility;

public class Actor_Health : ActorBehaviour
{
    [SerializeField]private float _maxHealth;
    public float health {get; private set;}
    public SecureEvent deathEvent = new SecureEvent();
    public SecureEvent instaKilledEvent = new SecureEvent();
    public bool isDead = false;
    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);
    }
    public override void InitializeBehaviour(Actor newActor)
    {
        base.InitializeBehaviour(newActor);
        health = _maxHealth;
    }
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
        if(health <= 0 && !isDead)
        {
            deathEvent.Invoke();
            isDead = true;
        }
    }
    public void LoseHealth(float damage)
    {
        health -= damage;
    }

    public void GainHealth(float healing)
    {
        health += healing;
    }
}
