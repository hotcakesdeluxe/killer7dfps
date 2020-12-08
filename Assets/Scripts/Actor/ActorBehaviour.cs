using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorBehaviour : MonoBehaviour
{
    public Actor actor { get; private set; }
    public bool initialized { get; protected set; }

    public virtual void InitializeBehaviour(Actor newActor)
    {
        AssignActorReferences(newActor);
        initialized = true;
    }

    public virtual void AssignActorReferences(Actor newActor)
    {
        actor = newActor;
    }

    protected T GetBehaviour<T>() where T : ActorBehaviour
    {
        return actor.GetBehaviour<T>();
    }
    
    public virtual void UpdateBehaviour() { }
    public virtual void LateUpdateBehaviour() { }
    public virtual void FixedUpdateBehaviour() { }
    public virtual void TerminateBehaviour() { }
    public virtual void CollisionEnter(Collision col) { }
    public virtual void CollisionExit(Collision col) { }
    public virtual void CollisionStay(Collision col) { }

}