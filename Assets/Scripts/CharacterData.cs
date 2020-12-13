using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PHL.Texto;

[CreateAssetMenu(fileName = "Character", menuName = "PHL/Character", order = 0)]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public TextoData characterNameTexto;
    public List<AnimationClip> talkAnimations;
}
