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
    private int _characterInputDisabledFrames;
    [HideInInspector]
    public Vector2 smoothInputAim => _rawInputAim;
    [HideInInspector]
    public Vector2 smoothInputMovement;
    public float movementSmoothingSpeed = 1f;
    [SerializeField] private float _timeBetweenShots;
    [HideInInspector]
    public bool isAiming = false;
    public bool isFiring = false;
    public bool isReloading = false;
    private bool _canShoot = true;
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
    public override void LateUpdateBehaviour()
    {
        base.LateUpdateBehaviour();
        if (_characterInputDisabledFrames > 0)
        {
            _characterInputDisabledFrames--;
        }
        if (_characterInputDisabledFrames > 0)
        {
            _playerInput.currentActionMap.Disable();
        }
        else
        {
            _playerInput.currentActionMap.Enable();
        }
    }
    public void OnMovement(InputAction.CallbackContext value)
    {
        _rawInputMovement = value.ReadValue<Vector2>();
    }
    public void OnFire(InputAction.CallbackContext value)
    {
        if (isAiming && value.started)
        {
            if (_canShoot)
            {
                _canShoot = false;
                StartCoroutine(TimerRoutine());
                _shooting.Fire();
                isFiring = true;
            }

        }
        if (value.canceled)
        {
            isFiring = false;
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
        }
    }
    public void OnReload(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            _shooting.Reload();
        }
    }
    public void OnScan(InputAction.CallbackContext value)
    {
        if (isAiming && value.started)
        {
            _shooting.Scan();
        }
    }
    private void CalculateMovementInputSmoothing()
    {
        smoothInputMovement = Vector2.Lerp(smoothInputMovement, _rawInputMovement, Time.deltaTime * movementSmoothingSpeed);
    }
    private IEnumerator TimerRoutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime <= _timeBetweenShots)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _canShoot = true;
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
    public void DisableCharacterInputForFrames(int frames)
    {
        _characterInputDisabledFrames = frames;
        _playerInput.currentActionMap.Disable();
    }

}
