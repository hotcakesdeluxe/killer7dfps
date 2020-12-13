using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Weakpoint : ActorBehaviour
{
    [SerializeField] private GameObject _weakpointPrefab;
    [SerializeField]private HitboxRoot _hitboxRoot;
    private GameObject _weakpoint;
    private Actor_EnemyAnimation _animation;
    private List<HumanBodyBones> _weakpointBones = new List<HumanBodyBones>();
    private Transform _weakpointTransform;
    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);
        _animation = GetBehaviour<Actor_EnemyAnimation>();
    }
    public override void InitializeBehaviour(Actor newActor)
    {
        base.InitializeBehaviour(newActor);
        _weakpointBones.Add(HumanBodyBones.Head);
        _weakpointBones.Add(HumanBodyBones.LeftUpperLeg);
        _weakpointBones.Add(HumanBodyBones.RightUpperLeg);
        _weakpointBones.Add(HumanBodyBones.LeftLowerLeg);
        _weakpointBones.Add(HumanBodyBones.RightLowerLeg);
        _weakpointBones.Add(HumanBodyBones.Spine);
        //_weakpointBones.Add(HumanBodyBones.Chest);
        //_weakpointBones.Add(HumanBodyBones.UpperChest);
        _weakpointBones.Add(HumanBodyBones.LeftShoulder);
        _weakpointBones.Add(HumanBodyBones.RightShoulder);
        _weakpointBones.Add(HumanBodyBones.LeftUpperArm);
        _weakpointBones.Add(HumanBodyBones.RightUpperArm);
        _weakpointBones.Add(HumanBodyBones.LeftLowerArm);
        _weakpointBones.Add(HumanBodyBones.RightLowerArm);
        _weakpointTransform = _animation.animator.GetBoneTransform(_weakpointBones[Random.Range(0, _weakpointBones.Count -1)]);
        _weakpoint = Instantiate(_weakpointPrefab, _weakpointTransform.position, Quaternion.identity, _weakpointTransform);
        _weakpoint.GetComponent<ParticleSystem>().Stop();
        _weakpoint.GetComponent<HitboxCollider>().Initialize(_hitboxRoot);

    }
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
    }
    public void RevealWeakpoint()
    {
        _weakpoint.GetComponent<ParticleSystem>().Play();
    }

}
