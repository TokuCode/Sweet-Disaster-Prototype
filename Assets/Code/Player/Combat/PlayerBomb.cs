using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerBomb : Feature
{
    private PlayerController _playerController;
    
    [FormerlySerializedAs("_throwChargeTime")]
    [Header("Throw Parameters")] 
    [SerializeField] private float _throwChargeTimeSeconds;
    public float ThrowChargeTimeSeconds => _throwChargeTimeSeconds;
    [SerializeField] private float _throwChargeTimer;
    public float ThrowChargeTimer => _throwChargeTimer;
    [SerializeField] private float _throwMinForce;
    [SerializeField] private float _throwMaxForce;
    [SerializeField] private bool _isThrowing;
    public bool IsThrowing => _isThrowing;

    [Header("Resource Management")] 
    [SerializeField] private float _cooldownTimeSeconds;
    [SerializeField] private float _cooldownTimer;
    [SerializeField] private bool _isOnCooldown;
    public bool IsOnCooldown => _isOnCooldown;
    [SerializeField] private int _bombCount;
    public int BombCount => _bombCount;

    [Header("Bomb Parameters")] 
    [SerializeField] private PoolManager _bombPool;
    [SerializeField] private float _explosionArea;
    [SerializeField] private float _explosionDamageInCenter;
    [SerializeField] private float _explosionDamageInBorder;
    [SerializeField] private float _knockbackLevelInCenter;
    [SerializeField] private float _knockbackLevelInBorder;

    public void OnThrowInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            StartThrowing();
        else if (context.canceled)
            EndThrowing();
    }
    
    public override void InitializeFeature(Controller controller)
    {
        _playerController = (PlayerController)controller;
        _playerController.OnThrowInputEvent += OnThrowInput;
    }

    public override void UpdateFeature(UpdateContext context)
    {
        if (context == UpdateContext.Update)
        {
            if(_cooldownTimer > 0) 
                _cooldownTimer -= Time.deltaTime;
            else if(_isOnCooldown)
                ResetThrow();
            
            if (_isThrowing) ThrowCharge();
        }
    }

    private void StartThrowing()
    {
        bool canThrowInternal = _bombCount > 0 && !_isOnCooldown && !_isThrowing;
        bool canThrowExternal = !_playerController.IsShooting && !_playerController.IsCrouching;
        if (canThrowInternal && canThrowExternal)
        {
            _isThrowing = true;
            _throwChargeTimer = 0f;
            _playerController.BlockMovement();
        }
    }

    private void EndThrowing()
    {
        if (!_isThrowing) return;
        ThrowAction();
        _bombCount--;
        
        _isThrowing = false;
        _cooldownTimer = _cooldownTimeSeconds;
        _isOnCooldown = true;
        
        _playerController.UnblockMovement();
    }

    private void ResetThrow() => _isOnCooldown = false;

    private void ThrowCharge()
    {
        if(_throwChargeTimer < _throwChargeTimeSeconds)
            _throwChargeTimer += Time.deltaTime;
        else 
            _throwChargeTimer = 0;
    }

    private void ThrowAction()
    {
        var position = _playerController.HandlePosition;
        var direction = _playerController.HandleDirection;
        var throwForce = Mathf.Lerp(_throwMinForce, _throwMaxForce, Mathf.Clamp01(_throwChargeTimer / _throwChargeTimeSeconds));

        var instance = _bombPool.Request();
        var bomb = instance.RenderObject.GetComponent<ObjectBomb>();
        
        bomb.transform.position = position;
        bomb.ExplosionArea = _explosionArea;
        bomb.ExplosionDamageInCenter = _explosionDamageInCenter;
        bomb.ExplosionDamageInBorder = _explosionDamageInBorder;
        bomb.KnockbackLevelInCenter = _knockbackLevelInCenter;
        bomb.KnockbackLevelInBorder = _knockbackLevelInBorder;
        
        bomb.AddImpulse(direction.normalized * throwForce);
    }
}
