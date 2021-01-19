using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PHL.Common.Utility;
using PHL.Common.GameEvents;

public class ConAct_Choice : ConversationAction
{
    [SerializeField] private CharacterData _characterData;
    [SerializeField] private CharacterData _characterSpeakingTo;
    [SerializeField] [TextArea(3, 20)] private string _text;
    [SerializeField] private List<string> _choiceList;
    public List<string> choiceList
    {
        get
        {
            return _choiceList;
        }
    }
    
    private Actor_PlayerInput _input;

    public SpeakEvent speakStartEvent { get; protected set; } = new SpeakEvent();
    public SecureEvent speakEndEvent { get; protected set; } = new SecureEvent();
    public int currentChoiceIndex { get; private set; }

    public CharacterData characterData
    {
        get
        {
            return _characterData;
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        _input = parentConversation.controlPlayer.GetBehaviour<Actor_PlayerInput>();

        Actor_Conversation speakingActor = Actor_Conversation.GetActorByCharacterData(_characterData);
        Actor_Conversation speakingToActor = Actor_Conversation.GetActorByCharacterData(_characterSpeakingTo);

        parentConversation.AddActor(speakingActor);
        parentConversation.AddActor(speakingToActor);

        //speakingActor.FaceCharacter(speakingToActor);
        //speakingToActor.FaceCharacter(speakingActor);
    }

    protected override IEnumerator ActionRoutine()
    {
        speakStartEvent.Invoke(_characterData, _text, parentConversation.conversationType);

        yield return 0f;
        
        while(true)
        {
            //set these up in playerinput
            if(_input.menuSubmit.triggered)
            {
                break;
            }
            //this doesn't fucking work at all idiot
            if(_input.menuUp.triggered)
            {
                currentChoiceIndex--;
            }
            
            if(_input.menuDown.triggered)
            {
                currentChoiceIndex++;
            }

            currentChoiceIndex = ExtraMath.Modulus(currentChoiceIndex, _choiceList.Count);

            yield return 0f;
        }
        
        yield return 0f;

        yield return StartCoroutine(base.ActionRoutine());
    }

    public override void StopAction()
    {
        base.StopAction();

        speakEndEvent.Invoke();
    }

    public override ConversationAction GetNextAction()
    {
        return _connections[currentChoiceIndex].connectedConversation;
    }

    public override int GetConnectionCount()
    {
        if(_choiceList == null)
        {
            _choiceList = new List<string>();
        }

        return _choiceList.Count;
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        for(int i = 0; i < _choiceList.Count; i++)
        {
            _connections[i].id = _choiceList[i];
        }
    }

    public override Color GetNodeColor()
    {
        Color color;
        ColorUtility.TryParseHtmlString("#F2E530", out color);
        return color;
    }
}
