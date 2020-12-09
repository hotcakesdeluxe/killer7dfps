using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Actor_PlayerInput : ActorBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    private Actor_CharacterController _characterController;
    private Actor_Animation _animation;
    private Actor_Shooting _shooting;
    private Vector2 _rawInputMovement;
    private Vector2 _rawInputAim;
    [HideInInspector]
    public Vector2 smoothInputAim => _rawInputAim;
    [HideInInspector]
    public Vector2 smoothInputMovement;
    public float movementSmoothingSpeed = 1f;
    [HideInInspector]
    public bool isAiming = false;
    private string currentControlScheme;
    public override void AssignActorReferences(Actor newActor)
    {
        base.AssignActorReferences(newActor);
        _characterController = GetBehaviour<Actor_CharacterController>();
        _shooting = GetBehaviour<Actor_Shooting>();
    }
    public override void InitializeBehaviour(Actor newActor)
    {
        base.InitializeBehaviour(newActor);
    }
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
        CalculateMovementInputSmoothing();
    }
    public void OnMovement(InputAction.CallbackContext value)
    {
        _rawInputMovement = value.ReadValue<Vector2>();
    }
    public void OnFire(InputAction.CallbackContext value)
    {
        if(isAiming && value.started)
        {
            _shooting.Fire();
        }
    }
    public void OnAim(InputAction.CallbackContext value)
    {
        _rawInputAim = value.ReadValue<Vector2>();
    }
    public void OnToggleAim(InputAction.CallbackContext value)
    {
       
        if (value.started)
        {
            isAiming = !isAiming;
            Debug.Log("aiming " + isAiming);
        }
        /*else if (value.canceled)
        {
            isAiming = false;
        }*/
    }
    public void OnScan(InputAction.CallbackContext value)
    {
        if(isAiming && value.started)
        {
            _shooting.Scan();
        }
    }
    private void CalculateMovementInputSmoothing()
    {
        smoothInputMovement = Vector2.Lerp(smoothInputMovement, _rawInputMovement, Time.deltaTime * movementSmoothingSpeed);
    }
    public void OnControlsChanged()
    {
        if (_playerInput.currentControlScheme != currentControlScheme)
        {
            currentControlScheme = _playerInput.currentControlScheme;
            Debug.Log(currentControlScheme);
            RemoveAllBindingOverrides();
        }
    }
    private void RemoveAllBindingOverrides()
    {
        InputActionRebindingExtensions.RemoveAllBindingOverrides(_playerInput.currentActionMap);
    }

}
