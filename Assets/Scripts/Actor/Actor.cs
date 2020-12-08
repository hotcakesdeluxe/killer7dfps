using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PHL.Common.Utility;

public class Actor : MonoBehaviour
{
    public class ActorEvent : SecureEvent<Actor> { }

    [SerializeField] protected bool _debug;
    public bool debug
    {
        get
        {
            return _debug;
        }
    }

    [SerializeField] private List<ActorBehaviour> _actorBehaviours;
    public List<ActorBehaviour> actorBehaviours
    {
        get
        {
            if(_actorBehaviours == null)
            {
                _actorBehaviours = new List<ActorBehaviour>();
            }

            return _actorBehaviours;
        }
    }

    private Dictionary<System.Type, ActorBehaviour> _actorBehaviourDictionary;
    public Dictionary<System.Type, ActorBehaviour> actorBehaviourDictionary
    {
        get
        {
            if(_actorBehaviourDictionary == null)
            {
                _actorBehaviourDictionary = new Dictionary<System.Type, ActorBehaviour>();
            }

            return _actorBehaviourDictionary;
        }
    }

    public T GetBehaviour<T>() where T : ActorBehaviour
    {
        if (actorBehaviourDictionary.ContainsKey(typeof(T)))
        {
            return (T)(actorBehaviourDictionary[typeof(T)]);
        }
        else
        {
            foreach (ActorBehaviour actorBehaviour in actorBehaviours)
            {
                if (actorBehaviour is T)
                {
                    return (T)actorBehaviour;
                }
            }
        }

        return null;
    }

    public void RefreshDictionary()
    {
        if (actorBehaviourDictionary != null)
        {
            actorBehaviourDictionary.Clear();

            foreach (ActorBehaviour actorBehaviour in actorBehaviours)
            {
                actorBehaviourDictionary.Add(actorBehaviour.GetType(), actorBehaviour);
            }
        }
    }

    public void AddBehaviour(ActorBehaviour actorBehaviour)
    {
        if (!actorBehaviours.Contains(actorBehaviour))
        {
            actorBehaviours.Add(actorBehaviour);
        }

        RefreshDictionary();

        if (!actorBehaviour.initialized)
        {
            actorBehaviour.InitializeBehaviour(this);
        }
        
        AssignActorReferences();
    }

    public void AddBehaviours(List<ActorBehaviour> behavioursToAdd)
    {
        foreach (ActorBehaviour actorBehaviour in behavioursToAdd)
        {
            if (!actorBehaviours.Contains(actorBehaviour))
            {
                actorBehaviours.Add(actorBehaviour);
            }

            if (!actorBehaviour.initialized)
            {
                actorBehaviour.InitializeBehaviour(this);
            }
        }

        AssignActorReferences();
    }

    public void RemoveBehaviour(ActorBehaviour actorBehaviour)
    {
        if(actorBehaviours.Contains(actorBehaviour))
        {
            actorBehaviours.Remove(actorBehaviour);
        }

        AssignActorReferences();
        actorBehaviour.TerminateBehaviour();
    }

    public void RemoveBehaviours(List<ActorBehaviour> behavioursToRemove)
    {
        foreach(ActorBehaviour actorBehaviour in behavioursToRemove)
        {
            if (actorBehaviours.Contains(actorBehaviour))
            {
                actorBehaviours.Remove(actorBehaviour);
            }

            actorBehaviour.TerminateBehaviour();
        }

        AssignActorReferences();
    }

    public void AssignActorReferences()
    {
        RefreshDictionary();

        for (int i = 0; i < actorBehaviours.Count; i++)
        {
            actorBehaviours[i].AssignActorReferences(this);
        }
    }

    public void InitializeActor()
    {
        RefreshDictionary();

        for(int i = 0; i < actorBehaviours.Count; i++)
        {
            if (!actorBehaviours[i].initialized)
            {
                actorBehaviours[i].InitializeBehaviour(this);
            }
        }
    }

    private void UpdateActor()
    {
        if (_debug)
        {
            for (int i = 0; i < actorBehaviours.Count; i++)
            {
                actorBehaviours[i].UpdateBehaviour();
            }
        }
        else
        {
            for (int i = 0; i < actorBehaviours.Count; i++)
            {
                actorBehaviours[i].UpdateBehaviour();
            }
        }
    }

    private void LateUpdateActor()
    {
        for (int i = 0; i < actorBehaviours.Count; i++)
        {
            actorBehaviours[i].LateUpdateBehaviour();
        }
    }

    private void FixedUpdateActor()
    {
        for (int i = 0; i < actorBehaviours.Count; i++)
        {
            actorBehaviours[i].FixedUpdateBehaviour();
        }
    }

    private void TerminateActor()
    {
        for (int i = 0; i < actorBehaviours.Count; i++)
        {
            actorBehaviours[i].TerminateBehaviour();
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        for (int i = 0; i < actorBehaviours.Count; i++)
        {
            actorBehaviours[i].CollisionEnter(collision);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        for (int i = 0; i < actorBehaviours.Count; i++)
        {
            actorBehaviours[i].CollisionExit(collision);
        }
    }
    
    private void OnCollisionStay(Collision collision)
    {
        for (int i = 0; i < actorBehaviours.Count; i++)
        {
            actorBehaviours[i].CollisionStay(collision);
        }
    }

    private void Awake()
    {
        AssignActorReferences();
        InitializeActor();
    }

    private void Update()
    {
        UpdateActor();
    }

    private void LateUpdate()
    {
        LateUpdateActor();
    }

    private void FixedUpdate()
    {
        FixedUpdateActor();
    }

    private void OnDestroy()
    {
        TerminateActor();
    }
}
