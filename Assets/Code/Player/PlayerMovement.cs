using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : Feature
{
    private PlayerController _playerController;

    [Header("Movement Parameters")]
    [SerializeField] private float _airMultiplier;
    
    [Header("Input")]
    [SerializeField] private float _horizontalInput;

    [Header("State Parameters")]
    [SerializeField] private bool _isMovementBlocked;
    public bool IsMovementBlocked => _isMovementBlocked;
    
    public void OnMoveInput(InputAction.CallbackContext context) => _horizontalInput = context.ReadValue<float>();

    public override void InitializeFeature(Controller controller)
    { 
        _playerController = (PlayerController)controller;   
        _playerController.OnMoveInputEvent += OnMoveInput;
    }

    public override void UpdateFeature(UpdateContext context)
    {
        if (context == UpdateContext.FixedUpdate)
        {
            Move();
            LimitSpeed();
        }
    }

    private void Move()
    {
        if(_isMovementBlocked) return;
        
        if (_horizontalInput != 0)
        {
            Vector2 direction = Vector2.right;
            if (_playerController.OnSlope && !_playerController.OnDeparture)
                direction = _playerController.ProjectOnSlope(direction);
            
            Vector2 movement = direction * (_horizontalInput * _playerController.Acceleration);
            float multiplier = _playerController.IsGrounded ? 1f : _airMultiplier;
            _playerController.AddForce(movement * multiplier);
        }
    }

    private void LimitSpeed()
    {
        float maxSpeed = _playerController.MaxSpeed;
        
        if (_playerController.OnSlope && !_playerController.OnDeparture)
        {
            if(_playerController.Velocity.magnitude > maxSpeed) 
                _playerController.Velocity = _playerController.Velocity.normalized * maxSpeed;
            return;
        }
        
        if (Mathf.Abs(_playerController.Velocity.x) > maxSpeed)
            _playerController.Velocity = new Vector2(Mathf.Sign(_playerController.Velocity.x) * maxSpeed, _playerController.Velocity.y);
    }
    
    public void BlockMovement() => _isMovementBlocked = true;
    public void UnblockMovement() => _isMovementBlocked = false;
}
