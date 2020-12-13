using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PHL.Common.Utility;
public class Interactable : MonoBehaviour
{
    [SerializeField] private InteractableData _interactableData;
    public InteractableData interactableData => _interactableData;
    [SerializeField] private Vector3 _uiOffset;
    public Vector3 uiOffset => _uiOffset;
    public Actor.ActorEvent interactEvent { get; private set; } = new Actor.ActorEvent();

    public virtual void TryInteract(Actor sourceActor)
    {
        if (CanInteract(sourceActor))
        {
            Interact(sourceActor);
        }
    }

    protected virtual void Interact(Actor sourceActor)
    {
        interactEvent.Invoke(sourceActor);
    }

    public virtual bool CanInteract(Actor sourceActor)
    {
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        DebugExtension.DrawPoint(transform.TransformPoint(_uiOffset), Color.white, 0.5f);
    }
}
