using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : Feature
{
    private PlayerController _playerController;

    [Header("Control")] 
    [SerializeField] private bool _active;
    public bool Active => _active;

    [Header("Shooting Parameters")]
    [SerializeField] private float _timeBetweenShots;
    [SerializeField] private int _burstCount;
    [SerializeField] private float _timeBetweenBursts;
    [SerializeField] private bool _holdToShoot;
    [SerializeField] private float _lastShotTime;
    public float LastShotTime => _lastShotTime;
    [SerializeField] private bool _isShooting;
    public bool IsShooting => _isShooting;
    
    [Header("Reloading")]
    [SerializeField] private float _realoadTime;
    [SerializeField] private float _reloadTimer;
    [SerializeField] private int _magazineSize;
    public int MagazineSize => _magazineSize;
    [SerializeField] private int _currentAmmo;
    public int CurrentAmmo => _currentAmmo;
    [SerializeField] private bool _isReloading;
    public bool IsReloading => _isReloading;
    
    [Header("Projectile Parameters")]
    [SerializeField] private PoolManager _projectilePool;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _bulletLifeTime;
    [SerializeField] private float _bulletDamage;
    [SerializeField] private int _bulletKnocbackLevel;

    [Header("Trajectory Parameters")]
    [SerializeField] private float _baseImprecision;
    [SerializeField] private float _imprecision;
    [SerializeField] private float _imprecisionToAngleFactor;
    [SerializeField] private float _airImprecision;
    [SerializeField] private float _movementImprecisionPerSpeedUnit;

    [Header("Input")] 
    [SerializeField] private bool _shootHoldInput;

    public void OnShootInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_holdToShoot) _shootHoldInput = true;
            else StartShootingEval();
        }
        else if (context.canceled && _holdToShoot) _shootHoldInput = false;
    }

    public void OnReloadInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            TryReload();
    }
    
    public override void InitializeFeature(Controller controller)
    {
        _playerController = (PlayerController)controller;
        _playerController.OnShootInputEvent += OnShootInput;
        _playerController.OnReloadInputEvent += OnReloadInput;
        
        _currentAmmo = _magazineSize;
    }

    public override void UpdateFeature(UpdateContext context)
    {
        if (context == UpdateContext.Update)
        {
            UpdateImprecision();
            if (_holdToShoot && _shootHoldInput)
                StartShootingEval();
            
            if(_reloadTimer > 0) _reloadTimer -= Time.deltaTime;
            else if(_isReloading) StopReloading();
        }
    }

    private void StartShootingEval()
    {
        bool canShootInternal = _currentAmmo > 0 & Time.time - _lastShotTime > _timeBetweenBursts & !_isShooting & !_isReloading && _active;
        bool canShootExternal = !_playerController.IsCrouching && !_playerController.IsThrowing &&
                                !_playerController.IsShieldActive && !_playerController.IsStunned;
        if (canShootInternal && canShootExternal)
            StartCoroutine(ShootingSequence());
        else if(_currentAmmo <= 0) TryReload();
    }

    private IEnumerator ShootingSequence()
    {
        _isShooting = true;
        
        for (int i = 0; i < _burstCount; i++)
        { 
            ShootAction();
            _lastShotTime = Time.time;
            _currentAmmo--;
            
            if(_currentAmmo <= 0) 
                break;
            
            yield return new WaitForSeconds(_timeBetweenShots);
        }
        
        _isShooting = false;
    }

    private void ShootAction()
    {
        var instance = _projectilePool.Request();
        var bullet = instance.RenderObject.GetComponent<ObjectBullet>();
        
        bullet.EnemyTag = "Enemy";
        bullet.transform.position = _playerController.HandlePosition;
        bullet.LifeTime = _bulletLifeTime;
        bullet.Damage = _bulletDamage;
        bullet.KnockbackLevel = _bulletKnocbackLevel;
        bullet.Speed = _bulletSpeed;
        bullet.Direction = ImprecisionDirection(_playerController.HandleDirection);
    }

    private void UpdateImprecision()
    {
        _imprecision = _baseImprecision;
        
        float movementImprecision = _playerController.Velocity.x * _movementImprecisionPerSpeedUnit;
        _imprecision += movementImprecision;
        
        if (!_playerController.IsGrounded)
            _imprecision += _airImprecision;
    }
    
    private Vector3 ImprecisionDirection(Vector3 direction)
    {
        float angleAmplitude = _imprecision * _imprecisionToAngleFactor;
        float randomAngle = Random.Range(-angleAmplitude, angleAmplitude);
        return Quaternion.Euler(0, 0, randomAngle) * direction;
    }

    private void TryReload()
    {
        if(!_isShooting && _currentAmmo < _magazineSize && !_isReloading)
        {
            _isReloading = true;
            _reloadTimer = _realoadTime;
        }
    }
    
    private void StopReloading()
    {
        ReloadAction();
        _isReloading = false;
    }
    
    private void ReloadAction() => _currentAmmo = _magazineSize;
    
    public void SetActive(bool active) => _active = active;
}
