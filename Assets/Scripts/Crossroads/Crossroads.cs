using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PHL.Common.Utility;

public class CrossroadsEvent : SecureEvent<Crossroads> { }
public class Crossroads : MonoBehaviour
{
    public Actor controlPlayer { get; private set; }
    private List<Actor_Conversation> _actors = new List<Actor_Conversation>();
    private IEnumerator _currentCrossroadsRoutine;
    public CrossroadsAction currentCrossroadsAction { get; private set; }
    //public CrossroadsActionEvent crossroadsActionEvent { get; private set; } = new CrossroadsctionEvent();
    public CrossroadsEvent crossroadsEndedEvent { get; private set; } = new CrossroadsEvent();
    public List<Actor_Conversation> actors => _actors;
    public void AddActor(Actor_Conversation actor)
    {
        if (actor != null && !_actors.Contains(actor))
        {
            _actors.Add(actor);
        }
    }
}
