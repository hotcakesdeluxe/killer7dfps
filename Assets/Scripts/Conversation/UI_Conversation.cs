using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PHL.Common.Utility;
using PHL.Texto;
using TMPro;
using UnityEngine.UI;

public class UI_Conversation : HUD_Element
{
    [SerializeField] private GameObject _speakObject;
    [SerializeField] private GameObject _speakerObject;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _speakText;
    [SerializeField] private List<GameObject> _choiceObjects;
    [SerializeField] private List<TextMeshProUGUI> _choiceTexts;
    [SerializeField] private List<Image> _choiceImages;
    [SerializeField] private GameObject _nextButton;
    private Actor_Conversation _actorConversation;
    private Conversation _currentConversation;
    private bool _speakWindowActive;
    private int _speakWindowActiveFrames;

    private TMP_FontAsset _baseFont_Name;
    private TMP_FontAsset _baseFont_Speak;
    private Material _baseMaterial_Name;
    private Material _baseMaterial_Speak;


    void Start()
    {
        _speakObject.SetActive(false);
        _speakerObject.SetActive(false);
        foreach (GameObject choiceObject in _choiceObjects)
        {
            choiceObject.SetActive(false);
        }
    }
    protected override void Update()
    {
        base.Update();

        bool choiceActive = false;

        if (_currentConversation != null)
        {
            if (_currentConversation.currentConversationAction != null)
            {
                if (_currentConversation.currentConversationAction is ConAct_Choice)
                {
                    choiceActive = true;
                    ConAct_Choice choiceAction = (ConAct_Choice)(_currentConversation.currentConversationAction);
                    _speakText.maxVisibleCharacters = _speakText.text.Length;
                    for (int i = 0; i < choiceAction.choiceList.Count; i++)
                    {
                        _choiceObjects[i].SetActive(true);
                        _choiceTexts[i].text = choiceAction.choiceList[i];
                        _choiceImages[i].color = i == choiceAction.currentChoiceIndex ? Color.yellow : Color.black;
                    }
                }
                else if (_currentConversation.currentConversationAction is ConAct_Speak)
                {
                    ConAct_Speak speakAction = (ConAct_Speak)(_currentConversation.currentConversationAction);

                    _speakText.maxVisibleCharacters = Mathf.RoundToInt(ExtraMath.Map(speakAction.currentVisibleCharactersRatio, 0, 1, 0, _speakText.text.Length));

                    if (speakAction.nextButtonVisible)
                    {
                        _nextButton.SetActive(true);
                    }
                }
            }
        }
        if (!choiceActive)
        {
            foreach (GameObject choiceObject in _choiceObjects)
            {
                choiceObject.SetActive(false);
            }
        }
        if (_speakWindowActive)
        {
            _speakWindowActiveFrames = 2;
        }
        else
        {
            if (_speakWindowActiveFrames > 0)
            {
                _speakWindowActiveFrames--;
            }
            else
            {
                _speakObject.SetActive(false);
            }
        }
    }
    public override void AssignActorBehaviours()
    {
        base.AssignActorBehaviours();

        _actorConversation = _currentActor.GetBehaviour<Actor_Conversation>();

        if (_actorConversation != null)
        {
            _actorConversation.startConversationEvent.AddListener(ConversationStarted);
        }
    }
    public void ConversationStarted(Conversation newConversation)
    {
        if (newConversation.conversationType == Conversation.ConversationType.MissionDialogue ||
        newConversation.conversationType == Conversation.ConversationType.AmbientDialogue)
        {
            _currentConversation = newConversation;
            foreach (ConversationAction conversationAction in _currentConversation.actions)
            {
                if (conversationAction is ConAct_Speak)
                {
                    ConAct_Speak speakAction = (ConAct_Speak)conversationAction;
                    speakAction.speakStartEvent.AddListener(SpeakStartReceived);
                    speakAction.speakEndEvent.AddListener(SpeakEndReceived);
                }
                else if (conversationAction is ConAct_Choice)
                {
                    ConAct_Choice choiceAction = (ConAct_Choice)conversationAction;
                    choiceAction.speakStartEvent.AddListener(SpeakStartReceived);
                    choiceAction.speakEndEvent.AddListener(SpeakEndReceived);
                }
            }
        }
    }
    private void SpeakStartReceived(CharacterData characterData, string text, Conversation.ConversationType conversationType)
    {
        if (characterData != null)
        {
            _speakerObject.SetActive(true);

            if (characterData.characterNameTexto == null)
            {
                _nameText.text = characterData.characterName;
            }
            else
            {
                _nameText.text = characterData.characterNameTexto;
            }
        }
        else
        {
            _speakerObject.SetActive(false);
            _nameText.text = "";
        }
        //TextoAutoAssigner.AssignFont(_nameText, _baseFont_Name, _baseMaterial_Name);
        //TextoAutoAssigner.AssignFont(_speakText, _baseFont_Speak, _baseMaterial_Speak);
        bool isMissionDialogue = conversationType == Conversation.ConversationType.MissionDialogue;
        Debug.Log(text);
        if (isMissionDialogue)
        {
            _speakText.text = text;
            
            _speakText.maxVisibleCharacters = 0;
        }


        _speakWindowActiveFrames = 2;
        _speakObject.SetActive(isMissionDialogue);
        _speakWindowActive = true;
        _nextButton.SetActive(!isMissionDialogue);
    }
    private void SpeakEndReceived()
    {
        _speakWindowActive = false;
        _speakWindowActiveFrames = 5;
    }
}
