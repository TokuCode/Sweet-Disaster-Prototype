using System;
using UnityEngine;

public class PlayerHealth : Feature
{
    private PlayerController _playerController;
    
    [Header("Health Parameters")]
    [SerializeField] private float _health;
    public float Health => _health;
    [SerializeField] private float _baseHealth;
    public float BaseHealth => _baseHealth;
    public float HealthRatio => _health / _baseHealth;
    
    [Header("Stun")]
    [SerializeField] private bool _isStunned;
    public bool IsStunned => _isStunned;
    [SerializeField] private float _stunMinDuration;
    [SerializeField] private float _stunDurationPerKnockback;
    [SerializeField] private float _stunTimer;
    
    public event EventHandler<OnHealthChangedEventArgs> OnHealthChanged;
    public Pipeline<AttackEvent> AttackPipeline { get; private set; }

    public override void InitializeFeature(Controller controller)
    {
        _playerController = (PlayerController)controller;
        AttackPipeline = new Pipeline<AttackEvent>();
    }

    public override void UpdateFeature(UpdateContext context)
    {
        if (context == UpdateContext.Update)
        {
            if(_stunTimer > 0) _stunTimer -= Time.deltaTime;
            else if(_isStunned) UnStun();
        }
    }

    private void Damage(float percentage)
    {
        _health += percentage * _baseHealth;
        OnHealthChangedEventArgs args = new ()
        {
            Health = _health,
            MaxHealth = _baseHealth,
            HealthRatio = _health / _baseHealth
        };
        OnHealthChanged?.Invoke(this, args);
    }

    private void Knockback(Vector3 direction, float knockbackForce)
    {
        float knockback = knockbackForce * HealthRatio * 10f;
        _playerController.AddImpulse(direction.normalized * knockback);
        
        float stunTime = _stunMinDuration + _stunDurationPerKnockback * knockback;
        Stun(stunTime);
    }

    public void Stun(float duration)
    {
        _isStunned = true;
        _stunTimer = duration;
        _playerController.BlockMovement();
    }

    public void UnStun()
    {
        _isStunned = false;
        _playerController.UnblockMovement();
    }
    
    public void Attack(AttackEvent attackEvent)
    {
        AttackEvent resultEvent = AttackPipeline.Process(attackEvent, out bool isHit);
        
        if (isHit)
        {
            Damage(attackEvent.DamagePercentage);
            var direction = transform.position - attackEvent.SourcePosition;
            Knockback(direction.normalized, KnockbackTable.Instance.GetKnockbackForce(attackEvent.KnockbackLevel));
        }
    }
}
