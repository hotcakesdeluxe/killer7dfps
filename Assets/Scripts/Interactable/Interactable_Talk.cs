using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Talk : Interactable
{
    [SerializeField] private GameObject _conversationPrefab;

    protected override void Interact(Actor sourceActor)
    {
        base.Interact(sourceActor);

        GameObject conversationInstance = Instantiate(_conversationPrefab, null) as GameObject;
        conversationInstance.GetComponent<Conversation>().StartConversation(sourceActor);
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
