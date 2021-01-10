using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PHL.Common.Utility;
using PHL.Texto;

public class SpeakEvent : SecureEvent<CharacterData, string, Conversation.ConversationType> { }

public class ConAct_Speak : ConversationAction
{

    [SerializeField] private CharacterData _characterData;
    public CharacterData characterData => _characterData;

    [SerializeField] private CharacterData _characterSpeakingTo;
    public CharacterData speakingTo => _characterSpeakingTo;
    [SerializeField] [TextArea(3, 20)] private string _text;
    [SerializeField] private TextoData _texto;
    public float currentVisibleCharactersRatio { get; protected set; }
    private float _charactersPerSecond = 30;
    private Actor_PlayerInput _input;
    private Actor_Conversation _speakingActor;
    private Actor_Conversation _speakingToActor;
    public bool nextButtonVisible { get; protected set; }
    public SpeakEvent speakStartEvent { get; protected set; } = new SpeakEvent();
    public SecureEvent speakEndEvent { get; protected set; } = new SecureEvent();

    public override void Initialize()
    {
        base.Initialize();
        _input = parentConversation.controlPlayer.GetBehaviour<Actor_PlayerInput>();
        _speakingActor = null;
        _speakingToActor = null;

        if (_characterData != null)
        {
            _speakingActor = Actor_Conversation.GetActorByCharacterData(_characterData);
            parentConversation.AddActor(_speakingActor);
        }

        if (_characterSpeakingTo != null)
        {
            _speakingToActor = Actor_Conversation.GetActorByCharacterData(_characterSpeakingTo);
            parentConversation.AddActor(_speakingToActor);
        }
    }
    protected override IEnumerator ActionRoutine()
    {
        nextButtonVisible = false;
        string finalText = _text;


        if (_texto != null)
        {
            finalText = _texto;
        }

        speakStartEvent.Invoke(_characterData, finalText, parentConversation.conversationType);

        yield return 0f;

        yield return new WaitForSeconds(0.25f);
        nextButtonVisible = true;

        float speakingDuration = 1f;


        speakingDuration = ExtraMath.Map(finalText.Length, 0, 100, 0, 5f);
        //Debug.Log("chillin "+ _input.inputSystem_Menu.confirm.onDown);


        if (parentConversation.conversationType == Conversation.ConversationType.MissionDialogue)
        {
            currentVisibleCharactersRatio = 0;

            while (currentVisibleCharactersRatio < 1)
            {
                currentVisibleCharactersRatio += (Time.deltaTime / ((float)finalText.Length)) * _charactersPerSecond;

                if (_input.isInteract)//_input.inputSystem_Menu.confirm.onDown)
                {
                    currentVisibleCharactersRatio = 1;
                }

                speakingDuration -= Time.deltaTime;
                yield return 0f;
            }

            currentVisibleCharactersRatio = 1;

            bool buttonPress = false;
            while (!buttonPress)
            {
                speakingDuration -= Time.deltaTime;
                
                if (_input.isInteract)//_input.inputSystem_Menu.confirm.onDown)
                {
                    buttonPress = true;
                }

                yield return 0f;
            }
        }
        else
        {
            yield return new WaitForSeconds(speakingDuration);
        }

        yield return 0f;



        yield return StartCoroutine(base.ActionRoutine());
    }
    public override void StartAction()
    {
        base.StartAction();
    }
    public override void StopAction()
    {
        base.StopAction();

        speakEndEvent.Invoke();
    }
    public override Color GetNodeColor()
    {
        Color color;
        ColorUtility.TryParseHtmlString("#F2E530", out color);
        return color;
    }
}
