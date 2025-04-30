using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrouching : Feature
{
    private PlayerController _playerController;
    
    [Header("Crouch Parameters")]
    [SerializeField] private float _crouchHeightMultiplier;
    [SerializeField] private float _initialYScale;
    [SerializeField] private float _initialXSize;
    [SerializeField] private float _initialYSize;
    
    [Header("Runtime")]
    [SerializeField] private bool _isCrouching;
    public bool IsCrouching => _isCrouching;
    public bool CanCrouch => _playerController.IsGrounded && !_playerController.OnDeparture;
    [SerializeField] private bool _startingCrouch;
    public bool StartingCrouch => _startingCrouch;

    [Header("Input")] 
    [SerializeField] private bool _crouchInput;
    
    public void OnCrouchInput(InputAction.CallbackContext context)
    {
        if (context.performed) _crouchInput = true;
        else if (context.canceled) _crouchInput = false;
    }
    
    public override void InitializeFeature(Controller controller)
    {
        _playerController = (PlayerController)controller;
        _initialYScale = _playerController.LocalScale.y;
        _initialXSize = _playerController.Size.x;
        _initialYSize = _playerController.Size.y;
        _playerController.OnCrouchInputEvent += OnCrouchInput;
    }

    public override void UpdateFeature(UpdateContext context)
    {
        if (context == UpdateContext.Update)
        {
            if (_startingCrouch && _playerController.IsGrounded)
                _startingCrouch = false;
            ManageCrouch();
        }
    }

    private void ManageCrouch()
    {
        if(!_isCrouching && _crouchInput && CanCrouch)
        {
            _isCrouching = true;
            _startingCrouch = true;
            CrouchAction();
        }
        else if(_isCrouching && !_crouchInput && !_playerController.HeadBlocked)
        {
            _isCrouching = false;
            UnCrouchAction();
        } 
        else if(_isCrouching && !CanCrouch && !_startingCrouch && !_playerController.HeadBlocked)
        {
            _isCrouching = false;
            UnCrouchAction();
        }
    }

    private void CrouchAction()
    {
        _playerController.LocalScale = new Vector2(_playerController.LocalScale.x, _initialYScale * _crouchHeightMultiplier);
        _playerController.Size = new Vector2(_initialXSize * _crouchHeightMultiplier, _initialYSize * _crouchHeightMultiplier);
    }

    private void UnCrouchAction()
    { 
        _playerController.LocalScale = new Vector2(_playerController.LocalScale.x, _initialYScale);  
        _playerController.Size = new Vector2(_initialXSize, _initialYSize);
    } 
}
