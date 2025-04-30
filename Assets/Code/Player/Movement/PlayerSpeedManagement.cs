using System.Collections;
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
    
    [Header("Speed Transition Parameters")]
    [SerializeField] private float _transitionTimeSeconds;
    [SerializeField] private float _desiredMaxSpeed;
    [SerializeField] private bool _enableTransition;
    
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
        _enableTransition = false;
        
        if (_playerController.IsMovementBlocked)
        {
            _movementState = MovementState.Blocked;
        }
        
        else if (_playerController.IsCrouching)
        {
            _movementState = MovementState.Crouching;
            _desiredMaxSpeed = _maxSpeedCrouching;
            _acceleration = _accelerationCrouching;
            _enableTransition = true;
        }
        
        else if(_playerController.OnSlope)
        {
            _movementState = MovementState.Sliding;
            _desiredMaxSpeed = _maxSpeedIdle;
            _acceleration = _accelerationIdle;
            _enableTransition = true;
        }
        
        else if (_playerController.IsGrounded)
        {
            _movementState = MovementState.Idle;
            _desiredMaxSpeed = _maxSpeedIdle;
            _acceleration = _accelerationIdle;
            _enableTransition = true;
        }
        
        else
        {
            _movementState = MovementState.OnAir;
            _desiredMaxSpeed = _maxSpeedIdle;
            _acceleration = _accelerationIdle;
            _enableTransition = true;
        }

        if (_desiredMaxSpeed != _maxSpeed && _enableTransition)
        {
            StopAllCoroutines();
            StartCoroutine(SpeedTransition());
        }
    }

    private IEnumerator SpeedTransition()
    {
        float time = 0;
        float startSpeed = _maxSpeed;
        while (time < _transitionTimeSeconds)
        {
            time += Time.deltaTime;
            _maxSpeed = Mathf.Lerp(startSpeed, _desiredMaxSpeed, time / _transitionTimeSeconds);
            yield return null;
        }
        
        _maxSpeed = _desiredMaxSpeed;
    }
}