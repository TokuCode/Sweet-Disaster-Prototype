using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Controller
{
    private PlayerControls _controls;
    
    [Header("Features")]
    [SerializeField] private PlayerGroundCheck _groundCheck;
    public bool IsGrounded => _groundCheck.IsGrounded;
    public float LastTimeOnGround => _groundCheck.LastTimeOnGround;
    public bool OnSlope => _groundCheck.OnSlope;
    public bool HeadBlocked => _groundCheck.HeadBlocked;
    public Vector2 ProjectOnSlope(Vector2 vec) => _groundCheck.ProjectOnSlopeDirection(vec);
    [SerializeField] private PlayerMovement _movement;
    public bool IsMovementBlocked => _movement.IsMovementBlocked;
    public void BlockMovement() => _movement.BlockMovement();
    public void UnblockMovement() => _movement.UnblockMovement();
    [SerializeField] private PlayerJump _jump;
    public bool OnDeparture => _jump.OnDeparture;
    [SerializeField] private PlayerFriction _friction;
    public bool IsTurning => _friction.IsTurning;
    [SerializeField] private PlayerCrouching _crouching;
    public bool IsCrouching => _crouching.IsCrouching;
    public bool StartingCrouch => _crouching.StartingCrouch;
    [SerializeField] private PlayerSpeedManagement _speed;
    public float MaxSpeed => _speed.MaxSpeed;
    public float Acceleration => _speed.Acceleration;
    
    [Header("References")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private CapsuleCollider2D _collider2D;
    
    public event Action<InputAction.CallbackContext> OnMoveInputEvent;
    public event Action<InputAction.CallbackContext> OnJumpInputEvent;
    public event Action<InputAction.CallbackContext> OnCrouchInputEvent;
    
    private void OnMoveInput(InputAction.CallbackContext context) => OnMoveInputEvent?.Invoke(context);
    private void OnJumpInput(InputAction.CallbackContext context) => OnJumpInputEvent?.Invoke(context);
    private void OnCrouchInput(InputAction.CallbackContext context) => OnCrouchInputEvent?.Invoke(context);
    
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<CapsuleCollider2D>();
        _groundCheck = GetComponent<PlayerGroundCheck>();
        _movement = GetComponent<PlayerMovement>();
        _jump = GetComponent<PlayerJump>();
        _friction = GetComponent<PlayerFriction>();
        _crouching = GetComponent<PlayerCrouching>();
        _speed = GetComponent<PlayerSpeedManagement>();
        
        _controls = new PlayerControls();
        
        _controls.Gameplay.Move.performed += OnMoveInput;
        _controls.Gameplay.Move.canceled += OnMoveInput;
        _controls.Gameplay.Jump.performed += OnJumpInput;
        _controls.Gameplay.Jump.canceled += OnJumpInput;
        _controls.Gameplay.Crouch.performed += OnCrouchInput;
        _controls.Gameplay.Crouch.canceled += OnCrouchInput;
        
        _features = new List<Feature>();
        _features.Add(_groundCheck);
        _features.Add(_movement);
        _features.Add(_jump);
        _features.Add(_friction);
        _features.Add(_crouching);
        _features.Add(_speed);
    }

    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();

    public Vector3 Position => transform.position;

    public Vector3 LocalScale
    {
        get => transform.localScale;
        set => transform.localScale = value;
    }

    public Vector2 Size
    {
        get => _collider2D.size;
        set => _collider2D.size = value;
    }

    public Vector2 Velocity
    {
        get => _rigidbody2D.linearVelocity;
        set => _rigidbody2D.linearVelocity = value;
    }

    public void AddForce(Vector2 force) => _rigidbody2D.AddForce(force);
    public void AddImpulse(Vector2 impulse) => _rigidbody2D.AddForce(impulse, ForceMode2D.Impulse);
}
