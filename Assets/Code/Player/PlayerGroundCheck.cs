using UnityEngine;

public class PlayerGroundCheck : Feature
{
    private const float _extraDistance = .01f;
    
    private PlayerController _playerController;
    
    [Header("Ground Check Parameters")]
    [SerializeField] private LayerMask _groundLayer;
    
    [Header("Slope Check Parameters")]
    [SerializeField] private float _maxSlopeAngle;
    private RaycastHit2D _slopeHit;
    
    [Header("Runtime")]
    [SerializeField] private bool _isGrounded;
    public bool IsGrounded => _isGrounded;
    [SerializeField] private float _lastTimeOnGround;
    public float LastTimeOnGround => _lastTimeOnGround;
    [SerializeField] private bool _onSlope;
    public bool OnSlope => _onSlope;
    [SerializeField] private bool _headBlocked;
    public bool HeadBlocked => _headBlocked;
    
    public override void InitializeFeature(Controller controller) => _playerController = (PlayerController)controller;

    public override void UpdateFeature(UpdateContext context)
    {
        if (context == UpdateContext.Update)
        {
            GroundCheck();  
            SlopeCheck();
            CheckHeadBlocked();
        }
    }

    private void GroundCheck()
    {
        var position = _playerController.Position;
        var size = _playerController.Size;

        var footSize = new Vector2(size.x / 2, _extraDistance);
        var distance = size.y / 2 + _extraDistance;

        RaycastHit2D hit2D = Physics2D.BoxCast(position, footSize, 0f, Vector2.down, distance, _groundLayer);
        
        _isGrounded = hit2D.collider != null;
        if(_isGrounded) _lastTimeOnGround = Time.time;
    }

    private void SlopeCheck()
    {
        var position = _playerController.Position;
        var size = _playerController.Size;
        var distance = size.y / 2 + _extraDistance;

        _slopeHit = Physics2D.Raycast(position, Vector2.down, distance, _groundLayer);

        if (_slopeHit.collider != null)
        {
            var slopeAngle = Vector2.Angle(Vector2.up, _slopeHit.normal);
            _onSlope = slopeAngle <= _maxSlopeAngle & slopeAngle != 0;
        }
        else _onSlope = false;
    }

    public Vector2 ProjectOnSlopeDirection(Vector2 vec)
    {
        if (!_onSlope) return vec;
        Vector2 tangent = Vector2.Perpendicular(_slopeHit.normal).normalized;
        return tangent * Vector2.Dot(tangent, vec);
    }

    private void CheckHeadBlocked()
    {
        if (!_playerController.IsCrouching)
        {
            _headBlocked = false;
            return;
        }
        
        var position = _playerController.Position;
        var size = _playerController.Size;

        var headSize = new Vector2(size.x / 2, _extraDistance);
        var distance = size.y * 1.5f + _extraDistance;

        RaycastHit2D hit2D = Physics2D.BoxCast(position, headSize, 0f, Vector2.up, distance, _groundLayer);
        
        _headBlocked = hit2D.collider != null;
    }
}
