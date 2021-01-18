using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PHL.Common.Utility;

public class InteractionEvent : SecureEvent<Interactable> { }
public class Actor_Interaction : ActorBehaviour
{
    [SerializeField] private Transform _interactionPoint;

    private Actor_PlayerInput _input;
    private Actor_Conversation _conversation;
    private IEnumerator _interactionRoutine;
    public Interactable hoverInteractable { get; private set; }
    public InteractionEvent interactEvent { get; private set; } = new InteractionEvent();
    public bool interacting
    {
        get
        {
            return _interactionRoutine != null;
        }
    }
    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);

        _input = GetBehaviour<Actor_PlayerInput>();
        _conversation = GetBehaviour<Actor_Conversation>();

    }
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();

        hoverInteractable = null;

        Collider[] overlaps = Physics.OverlapSphere(_interactionPoint.position, _interactionPoint.localScale.x, (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Actor")));
        List<Interactable> interactables = new List<Interactable>();

        foreach (Collider overlap in overlaps)
        {
            Interactable interactable = overlap.GetComponent<Interactable>();
            if (interactable != null && interactable.CanInteract(actor))
            {
                interactables.Add(interactable);
            }
        }

        if (interactables.Count > 0)
        {
            interactables.Sort((x, y) => Vector3.Distance(_interactionPoint.position, x.transform.position).CompareTo(Vector3.Distance(_interactionPoint.position, y.transform.position)));
            hoverInteractable = interactables[0];
        }
        if (CanInteract())
        {
            if (_input != null)
            {
               if (_input.isInteract && !_conversation.conversing)//_input.inputSystem_Character.interact.onDown)
                {
                    StartInteraction(hoverInteractable);
                }
            }

        }
    }
    public virtual bool CanInteract()
    {
        if(hoverInteractable == null)
        {
            return false;
        }
        if (interacting)
        {
            return false;
        }
        return true;
    }
    protected virtual void Interact(Interactable interactable)
    {
        interactEvent.Invoke(hoverInteractable);
        hoverInteractable.TryInteract(actor);
    }
    protected virtual void StartInteraction(Interactable interactable)
    {
        StopInteraction();

        _interactionRoutine = InteractionRoutine(interactable);
        StartCoroutine(_interactionRoutine);
    }

    protected virtual void StopInteraction()
    {
        if (interacting)
        {
           //stop movement? disable gameplay movement?
        }

        if (_interactionRoutine != null)
        {
            StopCoroutine(_interactionRoutine);
            _interactionRoutine = null;
        }
    }
    protected virtual IEnumerator InteractionRoutine(Interactable interactable)
    {
        Interact(interactable);

        float timer = 0;

        while (timer < 1f)
        {
            _input.DisableCharacterInputForFrames(10);
            //_input.DisableMenuInputForFrames(10);
            if (interactable.interactableData.animationType == InteractableData.AnimationType.None)
            {
                timer += Time.deltaTime / 0.5f;
            }
            yield return 0f;
        }
        StopInteraction();

    }

}
