using System.Collections;
using System.Collections.Generic;
using PHL.Common.GameEvents;
using UnityEngine;

public class ConversationReceiver : GameEventReceiver
{
    protected override void ReceiveEvent(GameEventInfo info)
    {
        base.ReceiveEvent(info);
       if (Actor_Player.players.Count > 0)
        {
            GameObject conversationInstance = Instantiate((GameObject)(info.GetObject("ConversationPrefab")), transform) as GameObject;
            conversationInstance.GetComponent<Conversation>().StartConversation(Actor_Player.players[0]);
        }
    }
}
