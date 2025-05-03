using UnityEngine;
using UnityEngine.Serialization;

public class ObjectBomb : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CircleCollider2D _collider2D;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private PoolInstanceBomb _poolInstanceBomb;
    [SerializeField] private GameObject _explosionEffect;
    
    [Header("Bomb Parameters")]
    [SerializeField] private float _maxSlopeAngle;
    [SerializeField] private float _explosionPersistenceTime;
    [SerializeField] private float _collisionSimetryCoefficient;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask _attackLayer;
    
    [Header("Bomb Exposed Parameters")]
    [SerializeField] private float _explosionRadius;
    public float ExplosionRadius
    {
        get => _explosionRadius;
        set => _explosionRadius = value;
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
    [SerializeField] private int _knockbackLevelInCenter;
    public int KnockbackLevelInCenter
    {
        get => _knockbackLevelInCenter;
        set => _knockbackLevelInCenter = value;
    }
    [SerializeField] private int _knockbackLevelInBorder;
    public int KnockbackLevelInBorder
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
        _collider2D.includeLayers = 0;
    }
    
    public void PoolDestroy() => _poolInstanceBomb.Reset();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var surfaceNormal = collision.GetContact(0).normal;
        float angle = Vector3.Angle(Vector3.up, surfaceNormal);
        bool horizontal = angle < _maxSlopeAngle;
        
        if(collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player") || horizontal) Explode(); 
        else Bounce(surfaceNormal);
    }

    private void Explode()
    {
        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        _explosionEffect.SetActive(true);
        _explosionEffect.transform.position = transform.position;
        _explosionEffect.transform.localScale = Vector3.one * _explosionRadius;

        Invoke(nameof(PoolDestroy), _explosionPersistenceTime);
        
        var colliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius, _attackLayer);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                PlayerController player = collider.gameObject.GetComponent<PlayerController>();
                if (player != null)
                {
                    float distance = Vector2.Distance(transform.position, player.transform.position);
                    float ratio = 1 - Mathf.Clamp01(distance / _explosionRadius);
                    float damagePercentage = Mathf.Lerp(_explosionDamageInBorder, _explosionDamageInCenter, ratio);
                    float knockbackLevel = Mathf.Lerp(_knockbackLevelInBorder, _knockbackLevelInCenter, ratio);
                    
                    player.Attack(new()
                    {
                        DamagePercentage = damagePercentage,
                        KnockbackLevel = Mathf.RoundToInt(knockbackLevel),
                        SourcePosition = transform.position
                    });
                }
            }
        }
    }

    private void Bounce(Vector2 normal)
    {
        var velocity = _rigidbody2D.linearVelocity;
        var reflection = Vector2.Reflect(velocity, normal);
        _rigidbody2D.linearVelocity = reflection * _collisionSimetryCoefficient;
        _collider2D.includeLayers = _playerLayer;
    }

    public void AddImpulse(Vector2 force) => _rigidbody2D.AddForce(force, ForceMode2D.Impulse);
}
