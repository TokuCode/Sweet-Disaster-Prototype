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
    [SerializeField] private PlayerHandle _handle;
    public Vector3 HandlePosition => _handle.HandlePosition;
    public Vector3 HandleDirection => _handle.HandleDirection;
    [SerializeField] private PlayerShoot _shooting;
    public bool IsShootingActive => _shooting.Active;
    public void SetShootActive(bool active) => _shooting.SetActive(active);
    public bool IsShooting => _shooting.IsShooting;
    public bool IsReloading => _shooting.IsReloading;
    public int MagazineSize => _shooting.MagazineSize;
    public int CurrentAmmo => _shooting.CurrentAmmo;
    public float LastShotTime => _shooting.LastShotTime;
    [SerializeField] private PlayerBomb _bomb;
    public bool IsBombActive => _bomb.Active;
    public void SetBombActive(bool active) => _bomb.SetActive(active);
    public bool IsThrowing => _bomb.IsThrowing;
    public bool IsThrowOnCooldown => _bomb.IsOnCooldown;
    public int BombCount => _bomb.BombCount;
    public float ThrowChargeTimeSeconds => _bomb.ThrowChargeTimeSeconds;
    public float ThrowChargeTimer => _bomb.ThrowChargeTimer;
    [SerializeField] private PlayerHealth _health;
    public float Health => _health.Health;
    public float BaseHealth => _health.BaseHealth;
    public float HealthRatio => _health.HealthRatio;
    public bool IsStunned => _health.IsStunned;
    public Pipeline<AttackEvent> AttackPipeline => _health.AttackPipeline;
    public event EventHandler<OnHealthChangedEventArgs> OnHealthChanged
    {
        add => _health.OnHealthChanged += value;
        remove => _health.OnHealthChanged -= value;
    }
    public void Attack(AttackEvent attackEvent) => _health.Attack(attackEvent);
    [SerializeField] private PlayerShield _shield;
    public bool IsShieldActive => _shield.IsShieldActive;
    public float ShieldStamina => _shield.CurrentShieldStamina;
    public float MaxShieldStamina => _shield.MaxShieldStamina;
    public bool IsStaminaDepleted => _shield.IsStaminaDepleted;
    
    
    [Header("References")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private CapsuleCollider2D _collider2D;
    [SerializeField] private Camera _mainCamera;
    
    public delegate void OnInputEvent(InputAction.CallbackContext context);
    public event OnInputEvent OnMoveInputEvent;
    public event OnInputEvent OnJumpInputEvent;
    public event OnInputEvent OnCrouchInputEvent;
    public event OnInputEvent OnAimInputEvent; 
    public event OnInputEvent OnShootInputEvent;
    public event OnInputEvent OnReloadInputEvent;
    public event OnInputEvent OnChangeWeaponInputEvent;
    public event OnInputEvent OnShieldInputEvent;
    
    private void OnMoveInput(InputAction.CallbackContext context) => OnMoveInputEvent?.Invoke(context);
    private void OnJumpInput(InputAction.CallbackContext context) => OnJumpInputEvent?.Invoke(context);
    private void OnCrouchInput(InputAction.CallbackContext context) => OnCrouchInputEvent?.Invoke(context);
    private void OnAimInput(InputAction.CallbackContext context) => OnAimInputEvent?.Invoke(context);
    private void OnShootInput(InputAction.CallbackContext context) => OnShootInputEvent?.Invoke(context);
    private void OnReloadInput(InputAction.CallbackContext context) => OnReloadInputEvent?.Invoke(context);
    private void OnChangeWeaponInput(InputAction.CallbackContext context) => OnChangeWeaponInputEvent?.Invoke(context);
    private void OnShieldInput(InputAction.CallbackContext context) => OnShieldInputEvent?.Invoke(context);
    
    private void Awake()
    {
        _controls = new PlayerControls();
        
        _controls.Gameplay.Move.performed += OnMoveInput;
        _controls.Gameplay.Move.canceled += OnMoveInput;
        _controls.Gameplay.Jump.performed += OnJumpInput;
        _controls.Gameplay.Jump.canceled += OnJumpInput;
        _controls.Gameplay.Crouch.performed += OnCrouchInput;
        _controls.Gameplay.Crouch.canceled += OnCrouchInput;
        _controls.Gameplay.Aim.performed += OnAimInput;
        _controls.Gameplay.Aim.canceled += OnAimInput;
        _controls.Gameplay.Shoot.performed += OnShootInput;
        _controls.Gameplay.Shoot.canceled += OnShootInput;
        _controls.Gameplay.Reload.performed += OnReloadInput;
        _controls.Gameplay.Reload.canceled += OnReloadInput;
        _controls.Gameplay.ChangeWeapon.performed += OnChangeWeaponInput;
        _controls.Gameplay.ChangeWeapon.canceled += OnChangeWeaponInput;
        _controls.Gameplay.Shield.performed += OnShieldInput;
        _controls.Gameplay.Shield.canceled += OnShieldInput;
        
        _features = new List<Feature>();
        _features.Add(_groundCheck);
        _features.Add(_movement);
        _features.Add(_jump);
        _features.Add(_friction);
        _features.Add(_crouching);
        _features.Add(_speed);
        _features.Add(_handle);
        _features.Add(_shooting);
        _features.Add(_bomb);
        _features.Add(_health);
        _features.Add(_shield);
    }

    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();

    public Vector3 CenterPosition => transform.position + Vector3.up * Size.y / 2;

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
    
    public float GravrityScale
    {
        get => _rigidbody2D.gravityScale;
        set => _rigidbody2D.gravityScale = value;
    }

    public Vector3 ScreenToWorldPoint(Vector2 screenPoint)
    { 
        var worldPoint = _mainCamera.ScreenToWorldPoint(screenPoint);   
        worldPoint.z = 0;
        return worldPoint;
    }

    public void AddForce(Vector2 force) => _rigidbody2D.AddForce(force);
    public void AddImpulse(Vector2 impulse) => _rigidbody2D.AddForce(impulse, ForceMode2D.Impulse);
}
