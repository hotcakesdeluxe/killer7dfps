using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PHL.Common.GameEvents;
using PHL.Common.Utility;

public class Actor_Conversation : ActorBehaviour
{
    protected static Dictionary<CharacterData, List<Actor_Conversation>> _allActors = new Dictionary<CharacterData, List<Actor_Conversation>>();

    public static Actor_Conversation GetActorByCharacterData(CharacterData characterData)
    {
        if (characterData == null)
        {
            return null;
        }

        if (_allActors.ContainsKey(characterData) && _allActors[characterData].Count >= 1)
        {
            if (_allActors[characterData].Count > 1)
            {
                Vector3 playerPosition = Vector3.zero;

                if (Actor_Player.players != null && Actor_Player.players.Count > 0)
                {
                    playerPosition = Actor_Player.players[0].transform.position;
                }

                _allActors[characterData].Sort((x, y) => Vector3.Distance(playerPosition, x.actor.transform.position).CompareTo(Vector3.Distance(playerPosition, y.actor.transform.position)));
            }

            return _allActors[characterData][0];
        }

        return null;
    }
    public class ConversationEvent : SecureEvent<Conversation> { }
    [SerializeField] protected CharacterData _characterData;
    private Actor_PlayerInput _input;
    private Actor_Camera _camera;
    private Actor_Animation _animation;
    private Conversation _currentConversation;
    public ConversationEvent startConversationEvent { get; protected set; } = new ConversationEvent();
    public ConversationEvent stopConversationEvent { get; protected set; } = new ConversationEvent();
    public ConversationActionEvent conversationActionEvent { get; protected set; } = new ConversationActionEvent();

    public CharacterData characterData => _characterData;
    public Conversation currentConversation => _currentConversation;
    public bool conversing => _currentConversation != null;

    public override void InitializeBehaviour(Actor newActor)
    {
        base.InitializeBehaviour(newActor);

        if (_allActors.ContainsKey(_characterData))
        {
            if (!_allActors[_characterData].Contains(this))
            {
                _allActors[_characterData].Add(this);
            }
        }
        else
        {
            _allActors.Add(_characterData, new List<Actor_Conversation>() { this });
        }
    }
    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);

        _input = GetBehaviour<Actor_PlayerInput>();
        _camera = GetBehaviour<Actor_Camera>();
        _animation = GetBehaviour<Actor_Animation>();

    }
    public virtual void StartConversation(Conversation conversation)
    {
        _currentConversation = conversation;
        startConversationEvent.Invoke(conversation);
        _currentConversation.conversationActionEvent.AddListener(ConversationActionReceived);
        //
        //Actor_Conversation otherActor = conversation.actors.Find(x => x.characterData != characterData);
    }
    public virtual void StopConversation()
    {
        if (conversing)
        {
            stopConversationEvent.Invoke(_currentConversation);
            _currentConversation.conversationActionEvent.RemoveListener(ConversationActionReceived);
            if (_input != null)
            {
                _input.DisableCharacterInputForFrames(2);
                _input.ToggleActionMap("gameplay");
            }
        }
        _currentConversation = null;
    }
    private void ConversationActionReceived(ConversationAction conversationAction)
    {
        conversationActionEvent.Invoke(conversationAction);
        //do stuff with camera?
    }
    private void QuitReceived()
    {
        _allActors.Clear();
    }

}
