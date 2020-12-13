using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PHL.Common.Utility;
using PHL.Texto;
using PHL.Common.GameEvents;
public class Actor_Player : ActorBehaviour
{
    public static List<Actor> players = new List<Actor>();
    public bool isLeadPlayer
    {
        get
        {
            if (players.Count == 0)
            {
                return true;
            }

            return players[0] == actor;
        }
    }
    public override void InitializeBehaviour(Actor newActor)
    {
        base.InitializeBehaviour(newActor);
        players.Add(actor);
    }
    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);

    }
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
    }
}
