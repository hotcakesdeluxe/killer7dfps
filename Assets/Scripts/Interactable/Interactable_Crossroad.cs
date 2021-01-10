using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Crossroad : Interactable
{
    [SerializeField] private GameObject _crossroadsPrefab;

    protected override void Interact(Actor sourceActor)
    {
        base.Interact(sourceActor);

        GameObject crossroadsInstance = Instantiate(_crossroadsPrefab, null) as GameObject;
        crossroadsInstance.GetComponent<Conversation>().StartConversation(sourceActor);
    }

    public override bool CanInteract(Actor sourceActor)
    {
        Actor_Player player = sourceActor.GetBehaviour<Actor_Player>();
        if(player != null)
        {
            if(!player.isLeadPlayer)
            {
                return false;
            }
        }

        return base.CanInteract(sourceActor);
    }
}
