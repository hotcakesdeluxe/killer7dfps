using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Camera : ActorBehaviour
{
    private Actor_PlayerInput _input;
    public Camera FirstPersonCam;
    public GameObject FPS_vcam;
    public GameObject ThirdPerson_vcam;
    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);
        _input = GetBehaviour<Actor_PlayerInput>();
    }
    public override void InitializeBehaviour(Actor newActor)
    {
        base.InitializeBehaviour(newActor);
    }
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
        CameraSwitch();
    }
    private void CameraSwitch()
    {
        if (_input.isAiming)
        {
            FPS_vcam.SetActive(true);
            ThirdPerson_vcam.SetActive(false);
            FirstPersonCam.enabled = true;

        }
        else
        {
            FPS_vcam.SetActive(false);
            ThirdPerson_vcam.SetActive(true);
            FirstPersonCam.enabled = false;
        }
    }
}
