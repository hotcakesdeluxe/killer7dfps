using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PathCreation;

public class Actor_Crossroads : ActorBehaviour
{
    [SerializeField]private EventSystem _eventSystem; 
    private Canvas _crossroadCanvas;
    private CanvasGroup _crossroadCanvasGroup;
    private PathsButtonsHolder _pathsButtonsHolder;
    private Actor_PlayerInput _input;
    private Actor_CharacterController _characterController;
    private bool isFaded = true;
    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);
        _input = GetBehaviour<Actor_PlayerInput>();
        _characterController = GetBehaviour<Actor_CharacterController>();
    }
    public override void InitializeBehaviour(Actor newActor)
    {
        base.InitializeBehaviour(newActor);
        
    }
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
    }
    [ContextMenu("DoFade")]
    public void FadeCanvas()
    {
        StartCoroutine(FadeCanvasRoutine(_crossroadCanvasGroup.alpha, isFaded ? 1 : 0));
        isFaded = !isFaded;
    }
    private IEnumerator FadeCanvasRoutine(float start, float end)
    {
        float fadeTime = 0;
        float duration = .66f;
        while(fadeTime < duration)
        {
            fadeTime += Time.deltaTime;
            _crossroadCanvasGroup.alpha = Mathf.Lerp(start, end, fadeTime/duration);
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        _pathsButtonsHolder = other.GetComponent<PathsButtonsHolder>();
        _crossroadCanvas = other.transform.GetChild(0).GetComponent<Canvas>();
        _crossroadCanvasGroup = _crossroadCanvas.GetComponent<CanvasGroup>();
        _eventSystem.firstSelectedGameObject = _pathsButtonsHolder.Buttons[0].gameObject;
        _eventSystem.SetSelectedGameObject(_pathsButtonsHolder.Buttons[0].gameObject);
        FadeCanvas();
        RegisterButtons();
    }
    private void RegisterButtons()
    {
        Debug.Log("registered buttons");
        for(int i =0; i < _pathsButtonsHolder.Buttons.Count; i++)
        {
            int x = i;//today i learned you need to copy a variable into a lamba expression otherwise the compile is mad 
            _pathsButtonsHolder.Buttons[x].onClick.AddListener(()=>OnPathButtonClick(_pathsButtonsHolder.Paths[x]));
        }
    }
    private void OnPathButtonClick(PathCreator path)
    {
        Debug.Log("clicky clicky");
        _characterController.pathCreator = path;
        _characterController.distanceTravelled = 0f;
        FadeCanvas();
    }
}
