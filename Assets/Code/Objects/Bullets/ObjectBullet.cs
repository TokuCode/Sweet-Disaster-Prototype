using System;
using UnityEngine;

public class ObjectBullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private PoolInstanceBullet _poolInstanceBullet;

    [Header("Bullet Parameters")]
    [SerializeField] private string _enemyTag;
    public string EnemyTag
    {
        get => _enemyTag;
        set => _enemyTag = value;
    }
    [SerializeField] private float _lifeTime;
    public float LifeTime
    {
        get => _lifeTime;
        set
        {
            _lifeTime = value;
            _lifeTimeTimer = _lifeTime;
        }
    }
    [SerializeField] private float _lifeTimeTimer;
    [SerializeField] private float _damage;
    public float Damage
    {
        get => _damage;
        set => _damage = value;
    }
    [SerializeField] private int _knockbackLevel;
    public int KnockbackLevel
    {
        get => _knockbackLevel;
        set => _knockbackLevel = value;
    }
    [SerializeField] private float _speed;
    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }
    [SerializeField] private Vector2 _direction;
    public Vector2 Direction
    {
        get => _direction;
        set => _direction = value;
    }

    public void PoolReset()
    { 
        _lifeTimeTimer = _lifeTime;
        _direction = Vector2.zero;
        _rigidbody2D.linearVelocity = Vector2.zero;
        transform.localPosition = Vector2.zero;
        _enemyTag = string.Empty;
    }
    
    private void Update()
    {
        _lifeTimeTimer -= Time.deltaTime;
        if (_lifeTimeTimer <= 0)
            PoolDestroy();
    }

    private void FixedUpdate()
    {
        if(_direction != Vector2.zero) _rigidbody2D.linearVelocity = _direction.normalized * _speed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && _enemyTag.Equals("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Attack(new ()
                {
                    DamagePercentage = Damage,
                    KnockbackLevel = KnockbackLevel,
                    SourcePosition = transform.position
                });
                PoolDestroy();
            }
        }
        
        PoolDestroy();
    }

    public void PoolDestroy() => _poolInstanceBullet.Reset();
}
