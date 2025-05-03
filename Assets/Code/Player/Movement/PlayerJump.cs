using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : Feature
{
    private PlayerController _playerController;
    
    [Header("Jump Parameters")]
    [SerializeField] private float _jumpImpulse;

    [Header("Time Management")] 
    [SerializeField] private float _jumpCooldownSeconds;
    [SerializeField] private float _jumpCooldownTimer;
    
    [Header("Coyote Jump Parameters")]
    [SerializeField] private float _coyoteJumpTimeTolerance;
    
    [Header("Better Jump Parameters")]
    [SerializeField] private float _fallGravityMultiplier;
    [SerializeField] private float _lowJumpGravityMultiplier;
    [SerializeField] private bool _jumpHold;

    [Header("Runtime")] 
    [SerializeField] private bool _onDeparture;
    public bool OnDeparture => _onDeparture;
    public bool CanJump => !_playerController.IsCrouching && !_playerController.IsThrowing;

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Jump();
            _jumpHold = true;
        }
        else if (context.canceled) _jumpHold = false;
    }
    
    public override void InitializeFeature(Controller controller)
    {
        _playerController = (PlayerController)controller;
        _playerController.OnJumpInputEvent += OnJumpInput;
    }

    public override void UpdateFeature(UpdateContext context)
    {
        if (context == UpdateContext.Update)
        {
            if(_jumpCooldownTimer > 0) _jumpCooldownTimer -= Time.deltaTime;
            else if(_playerController.IsGrounded) _onDeparture = false;
            
            _playerController.GravrityScale = _playerController.OnSlope ? 0f : 1f;
        }
        else if (context == UpdateContext.FixedUpdate)
            BetterJump();
    }

    private void Jump()
    {
        if(_jumpCooldownTimer > 0 || !CanJump) return;
        
        float timeSinceOnGround = Time.time - _playerController.LastTimeOnGround;
        if(timeSinceOnGround > _coyoteJumpTimeTolerance) return;
        
        JumpAction();
        _jumpCooldownTimer = _jumpCooldownSeconds;
        _onDeparture = true;
    }
    
    private void JumpAction() => _playerController.AddImpulse(Vector2.up * _jumpImpulse);

    private void BetterJump()
    {
        if(_playerController.IsGrounded && !_playerController.OnSlope) return;
        
        if(_playerController.Velocity.y < 0)
            _playerController.AddImpulse(Vector2.up * Physics2D.gravity.y * (_fallGravityMultiplier - 1) * Time.fixedDeltaTime);
        else if(_playerController.Velocity.y > 0 && !_jumpHold)
            _playerController.AddImpulse(Vector2.up * Physics2D.gravity.y * (_lowJumpGravityMultiplier - 1) * Time.fixedDeltaTime);
    }
}
