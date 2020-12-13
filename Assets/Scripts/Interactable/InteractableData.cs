using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractableData", menuName = "PHL/Interactable", order = 0)]
public class InteractableData : ScriptableObject
{
    public enum AnimationType
    {
        None,
        Press,
        Grab
    }

    public string actionName;
    public Sprite sprite;
    public AnimationType animationType;
}
