using System;
using UnityEngine;

public class ObjectBomb : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private PoolInstanceBomb _poolInstanceBomb;
    [SerializeField] private GameObject _explosionEffect;
    
    [Header("Bomb Parameters")]
    [SerializeField] private float _maxSlopeAngle;
    [SerializeField] private float _explosionPersistenceTime;
    [SerializeField] private float _collisionSimetryCoefficient;
    
    [Header("Bomb Exposed Parameters")]
    [SerializeField] private float _explosionArea;
    public float ExplosionArea
    {
        get => _explosionArea;
        set => _explosionArea = value;
    }
    [SerializeField] private float _explosionDamageInCenter;
    public float ExplosionDamageInCenter
    {
        get => _explosionDamageInCenter;
        set => _explosionDamageInCenter = value;
    }
    [SerializeField] private float _explosionDamageInBorder;
    public float ExplosionDamageInBorder
    {
        get => _explosionDamageInBorder;
        set => _explosionDamageInBorder = value;
    }
    [SerializeField] private float _knockbackLevelInCenter;
    public float KnockbackLevelInCenter
    {
        get => _knockbackLevelInCenter;
        set => _knockbackLevelInCenter = value;
    }
    [SerializeField] private float _knockbackLevelInBorder;
    public float KnockbackLevelInBorder
    {
        get => _knockbackLevelInBorder;
        set => _knockbackLevelInBorder = value;
    }

    public void PoolReset()
    {
        _explosionEffect.transform.localScale = Vector3.one;
        _explosionEffect.SetActive(false);
        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.constraints = RigidbodyConstraints2D.None;
    }
    
    public void PoolDestroy() => _poolInstanceBomb.Reset();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var surfaceNormal = collision.GetContact(0).normal;
        float angle = Vector3.Angle(Vector3.up, surfaceNormal);
        bool horizontal = angle < _maxSlopeAngle;
        
        if(collision.gameObject.CompareTag("Enemy") || horizontal) Explode(); 
        else Bounce(surfaceNormal);
    }

    private void Explode()
    {
        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        _explosionEffect.SetActive(true);
        _explosionEffect.transform.position = transform.position;
        _explosionEffect.transform.localScale = Vector3.one * _explosionArea;

        Invoke(nameof(PoolDestroy), _explosionPersistenceTime);
    }

    private void Bounce(Vector2 normal)
    {
        var velocity = _rigidbody2D.linearVelocity;
        var reflection = Vector2.Reflect(velocity, normal);
        _rigidbody2D.linearVelocity = reflection * _collisionSimetryCoefficient;
    }

    public void AddImpulse(Vector2 force) => _rigidbody2D.AddForce(force, ForceMode2D.Impulse);
}
