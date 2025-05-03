using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFriction : Feature
{
    private PlayerController _playerController;

    [Header("Friction Parameters")]
    [SerializeField] private float _groundFriction;
    [SerializeField] private float _airFriction;

    [Header("Input")] 
    [SerializeField] private float _horizontalInput;
    public bool IsTurning => _horizontalInput > 0 && _playerController.Velocity.x < 0 ||
                              _horizontalInput < 0 && _playerController.Velocity.x > 0;
    [SerializeField] private bool _applyingFriction;
    
    public void OnMoveInput(InputAction.CallbackContext context) => _horizontalInput = context.ReadValue<float>();
    
    public override void InitializeFeature(Controller controller)
    {
        _playerController = (PlayerController)controller;
        _playerController.OnMoveInputEvent += OnMoveInput;
    }
    
    public override void UpdateFeature(UpdateContext context)
    {
        if (context == UpdateContext.FixedUpdate)
            ManageFriction();
    }

    private void ManageFriction()
    {
        _applyingFriction = false;

        if (_playerController.IsStunned) return;
        
        if(!_playerController.IsGrounded)
            ApplyFriction(_airFriction);
        else if(IsTurning || _horizontalInput == 0)
            ApplyFriction(_groundFriction);
    }

    private void ApplyFriction(float friction)
    {
        var velocity = _playerController.Velocity;
        _playerController.AddImpulse(-velocity * (friction * Time.fixedDeltaTime));
        _applyingFriction = true;
    }
}
