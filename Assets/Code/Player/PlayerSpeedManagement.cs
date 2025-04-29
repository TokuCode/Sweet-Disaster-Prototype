using UnityEngine;

public class PlayerSpeedManagement : Feature
{
    public enum MovementState
    {
        Idle,
        OnAir,
        Sliding,
        Crouching,
        Blocked
    }
    
    private PlayerController _playerController;
    
    [Header("Movement State Parameters")]
    [SerializeField] private MovementState _movementState;
    
    [Header("Speed Parameters")]
    [SerializeField] private float _maxSpeedIdle;
    [SerializeField] private float _accelerationIdle;
    [SerializeField] private float _maxSpeedCrouching;
    [SerializeField] private float _accelerationCrouching;
    
    [Header("Runtime")]
    [SerializeField] private float _maxSpeed;
    public float MaxSpeed => _maxSpeed;
    [SerializeField] private float _acceleration;
    public float Acceleration => _acceleration;


    public override void InitializeFeature(Controller controller) => _playerController = (PlayerController)controller;

    public override void UpdateFeature(UpdateContext context)
    {
        if(context == UpdateContext.Update) 
            SpeedManagement();
    }

    private void SpeedManagement()
    {
        if (_playerController.IsMovementBlocked)
        {
            _movementState = MovementState.Blocked;
        }
        
        else if (_playerController.IsCrouching)
        {
            _movementState = MovementState.Crouching;
            _maxSpeed = _maxSpeedCrouching;
            _acceleration = _accelerationCrouching;
        }
        
        else if(_playerController.OnSlope)
        {
            _movementState = MovementState.Sliding;
            _maxSpeed = _maxSpeedIdle;
            _acceleration = _accelerationIdle;
        }
        
        else if (_playerController.IsGrounded)
        {
            _movementState = MovementState.Idle;
            _maxSpeed = _maxSpeedIdle;
            _acceleration = _accelerationIdle;
        }
        
        else
        {
            _movementState = MovementState.OnAir;
            _maxSpeed = _maxSpeedIdle;
            _acceleration = _accelerationIdle;
        }
    }
}