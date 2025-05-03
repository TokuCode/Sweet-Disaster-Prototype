using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShield : Feature
{
    private PlayerController _playerController;
    
    [Header("Shield Parameters")]
    [SerializeField] private GameObject _shield;
    [SerializeField] private float _shieldAngle;
    [SerializeField] private bool _isShieldActive;
    [SerializeField] private float _deactivateShieldDelay;
    [SerializeField] private bool _isDeactivatingShield;
    public bool IsShieldActive => _isShieldActive || _isDeactivatingShield;

    [Header("Shield Stamina")] 
    [SerializeField] private float _maxShieldStamina;
    public float MaxShieldStamina => _maxShieldStamina;
    [SerializeField] private float _currentShieldStamina;
    public float CurrentShieldStamina => _currentShieldStamina;
    [SerializeField] private float _shieldStaminaRegenRate;
    [SerializeField] private float _shieldStaminaSpendRate;
    [SerializeField] private bool _isStaminaDepleted;
    public bool IsStaminaDepleted => _isStaminaDepleted;
    [SerializeField] private float _minShieldStaminaForActivation;
    
    [Header("Input")]
    [SerializeField] private bool _shieldInput;

    private Process<AttackEvent> repelAttackProcess;
    
    public void OnShieldInput(InputAction.CallbackContext context) 
    {
        if (context.performed)
        {
            _shieldInput = true;
            if (!_isShieldActive) TryActivateShield();
        }
        else if (context.canceled)
        {
            _shieldInput = false;
            if (_isShieldActive) TryDeactivateShield();
        }
    }
    
    public override void InitializeFeature(Controller controller)
    {
        _playerController = (PlayerController)controller;
        _playerController.OnShieldInputEvent += OnShieldInput;
        _playerController.OnHealthChanged += OnHealthChanged;
        repelAttackProcess = new (1, @event => @event, @event => RepelAttack(@event));
        _playerController.AttackPipeline.Register(repelAttackProcess);
        _shield.SetActive(false);
        _currentShieldStamina = _maxShieldStamina;
    }

    public override void UpdateFeature(UpdateContext context)
    {
        if (context == UpdateContext.Update)
        {
            if(_isShieldActive) WhileShieldActive();
            StaminaManagement();
        }
    }

    public void TryActivateShield()
    {
        bool canShieldInternal = !_isStaminaDepleted && !_isShieldActive && _currentShieldStamina > _minShieldStaminaForActivation && !_isDeactivatingShield;
        bool canShieldExternal = !_playerController.IsShooting && !_playerController.IsReloading &&
                                 !_playerController.IsCrouching && !_playerController.IsThrowing &&
                                 !_playerController.IsStunned;
        
        if (canShieldInternal && canShieldExternal)
        {
            ActivateShield();
            _isShieldActive = true;
        }
    }

    public void TryDeactivateShield()
    {
        _isShieldActive = false;
        _isDeactivatingShield = true;
        
        Invoke(nameof(DeactivateShield), _deactivateShieldDelay);
    }
    
    public void ActivateShield()
    {
        _shield.SetActive(true);
        _playerController.BlockMovement();
    }
    
    public void DeactivateShield()
    {
        _isDeactivatingShield = false;
        _shield.SetActive(false);
        if(!_playerController.IsStunned) _playerController.UnblockMovement();
    }
    
    public void WhileShieldActive()
    {
        _shield.transform.position = _playerController.HandlePosition;
        _shield.transform.right = _playerController.HandleDirection;
    }

    public void StaminaManagement()
    {
        if (_isShieldActive)
        {
            if(_currentShieldStamina > 0) 
                _currentShieldStamina -= _shieldStaminaSpendRate * Time.deltaTime;
            else
            {
                _isStaminaDepleted = true;
                TryDeactivateShield();
            }
        }
        else
        {
            if(_currentShieldStamina < _maxShieldStamina) 
                _currentShieldStamina += _shieldStaminaRegenRate * Time.deltaTime;
            else
                _isStaminaDepleted = false;
        }
    }
    
    private bool RepelAttack(AttackEvent attack)
    {
        if (!_isShieldActive) return true;
        
        var direction = _playerController.HandleDirection;
        var diff = attack.SourcePosition - _playerController.HandlePosition;
        var angle = Vector3.Angle(direction, diff);
        
        return angle > _shieldAngle;
    }

    private void OnHealthChanged(object sender, OnHealthChangedEventArgs args)
    {
        if (_isShieldActive)
        {
            _isShieldActive = false;
            DeactivateShield();
        }
    }
}
