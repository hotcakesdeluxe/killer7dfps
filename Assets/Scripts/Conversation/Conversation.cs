using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PHL.Common.Utility;

public class ConversationEvent : SecureEvent<Conversation> { }
public class Conversation : MonoBehaviour
{
    public enum ConversationType
    {
        MissionDialogue,
        AmbientDialogue
    }
    [SerializeField] private ConversationType _conversationType = ConversationType.MissionDialogue;
    public ConversationType conversationType => _conversationType;
    public List<ConversationAction> actions
    {
        get
        {
            return new List<ConversationAction>(GetComponentsInChildren<ConversationAction>());
        }
    }
    public Actor controlPlayer { get; private set; }
    private List<Actor_Conversation> _actors = new List<Actor_Conversation>();
    private IEnumerator _currentConversationRoutine;
    public ConversationAction currentConversationAction { get; private set; }
    public ConversationActionEvent conversationActionEvent { get; private set; } = new ConversationActionEvent();
    public ConversationEvent conversationEndedEvent { get; private set; } = new ConversationEvent();
    public List<Actor_Conversation> actors => _actors;

    public void AddActor(Actor_Conversation actor)
    {
        if (actor != null && !_actors.Contains(actor))
        {
            _actors.Add(actor);
        }
    }
    public void StartConversation(Actor newControlPlayer)
    {
        foreach (Actor player in Actor_Player.players)
        {
            Actor_Conversation actorConversation = player.GetBehaviour<Actor_Conversation>();
            if (actorConversation != null)
            {
                if (actorConversation.conversing)
                {
                    actorConversation.currentConversation.StopConversation();
                }
            }
        }

        _actors = new List<Actor_Conversation>();
        controlPlayer = newControlPlayer;

        Actor_Conversation controlActor = newControlPlayer.GetBehaviour<Actor_Conversation>();
        if (controlActor != null && !_actors.Contains(controlActor))
        {
            _actors.Add(controlActor);
        }
        _currentConversationRoutine = ConversationRoutine();
        StartCoroutine(_currentConversationRoutine);
    }

    public void StopConversation()
    {
        if (_currentConversationRoutine != null)
        {
            StopCoroutine(_currentConversationRoutine);
            _currentConversationRoutine = null;
        }

        currentConversationAction = null;
        Debug.Log("this conversation is over!");
        conversationEndedEvent.Invoke(this);

        //Destroy(gameObject);
    }

    private IEnumerator ConversationRoutine()
    {
        if (conversationType == ConversationType.MissionDialogue)
        {
            yield return new WaitForSeconds(0.225f);
        }
        else
        {
            yield return 0f;
        }

        foreach (ConversationAction action in actions)
        {
            action.Initialize();
        }
        foreach (Actor_Conversation actor in _actors)
        {
            actor.StartConversation(this);
        }

        if (actions.Count > 0)
        {
            Debug.Log(actions.Count + " actions total");
            currentConversationAction = actions[0];

            while (currentConversationAction != null)
            {
                currentConversationAction.StartAction();
                conversationActionEvent.Invoke(currentConversationAction);

                while (currentConversationAction.actionRunning)
                {
                    yield return 0f;

                    if (conversationType == ConversationType.MissionDialogue)
                    {

                    }
                }
                currentConversationAction = currentConversationAction.GetNextAction();
                Debug.Log(currentConversationAction);
            }
        }

        StopConversation();
        foreach (Actor_Conversation actor in _actors)
        {
            actor.StopConversation();
        }
    }
}
