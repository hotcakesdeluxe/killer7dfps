using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using PathCreation;
using PHL.Common.Utility;
public class Actor_CharacterController : ActorBehaviour
{
    private Actor_PlayerInput _input;
    private Actor_Camera _camera;
    [SerializeField] private GameObject _modelPrefab;
    public GameObject modelPrefab => _modelPrefab;
    public PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction;
    public double distanceOnPath { get; private set; } //0-1 value
    public float distanceTravelled;
    [SerializeField] private float _movementSpeed;
    private float _distanceFromPoint;
    public float forwardAmount {get; private set;}
    public bool directionFlipped { get; private set; }
    private bool _pastCrossroadsPoint = false;
    private Vector3 _flipFacing = new Vector3(0,180,0);
    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);
        _input = GetBehaviour<Actor_PlayerInput>();
        _camera = GetBehaviour<Actor_Camera>();
    }
    public override void InitializeBehaviour(Actor newActor)
    {
        base.InitializeBehaviour(newActor);
        if (pathCreator != null)
        {
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            pathCreator.pathUpdated += OnPathChanged;
            _modelPrefab.transform.SetParent(null);
        }
    }
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
        distanceOnPath = distanceTravelled / pathCreator.path.length;
    }
    public override void FixedUpdateBehaviour()
    {
        base.FixedUpdateBehaviour();
        PlayerMovement();
        PlayerRotate();
    }
    private void PlayerMovement()
    {
        if (pathCreator != null)
        {
            if (!_input.isAiming && !_camera.isHit)
            {
                forwardAmount = _input.smoothInputMovement.y * _movementSpeed;
                distanceTravelled += forwardAmount;
                actor.transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                actor.transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                if (distanceTravelled < 0)
                {
                    distanceTravelled = 0;
                }
                if (distanceTravelled > pathCreator.path.length)
                {
                    distanceTravelled = pathCreator.path.length;
                }
                if (forwardAmount < 0)
                {
                    directionFlipped = true;
                }
                else
                {
                    directionFlipped = false;
                }
                _modelPrefab.transform.position = new Vector3(actor.transform.position.x,actor.transform.position.y,actor.transform.position.z);

            }

        }
    }
    private void PlayerRotate()
    {
        if (directionFlipped == true)
        {
            _modelPrefab.transform.eulerAngles = new Vector3(actor.transform.eulerAngles.x,actor.transform.eulerAngles.y -180, actor.transform.eulerAngles.z);
        }
        else
        {
           _modelPrefab.transform.rotation = actor.transform.rotation;
        }
    }
    void OnPathChanged()
    {
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }
}
