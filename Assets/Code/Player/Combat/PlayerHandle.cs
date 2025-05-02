using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHandle : Feature
{
    private PlayerController _playerController;

    [Header("Handle Parameters")]
    [SerializeField] private float _handleDistance;
    [SerializeField] private float _handleHeight;
    
    [Header("Input")] 
    [SerializeField] private Vector2 _mousePosition;
    
    [Header("Runtime")]
    [SerializeField] private Vector3 _handlePosition;
    public Vector3 HandlePosition => _handlePosition;
    [SerializeField] private Vector3 _handleDirection;
    public Vector3 HandleDirection => _handleDirection;

    public void OnAimInput(InputAction.CallbackContext context) => _mousePosition = context.ReadValue<Vector2>();

    public override void InitializeFeature(Controller controller)
    {
        _playerController = (PlayerController)controller;
        _playerController.OnAimInputEvent += OnAimInput;
    }

    public override void UpdateFeature(UpdateContext context)
    {
        if(context == UpdateContext.Update)
            CalculateHandlePosition();
    }

    private void CalculateHandlePosition()
    {
        var playerPosition = _playerController.CenterPosition + Vector3.up * _handleHeight;
        var mousePositionWorld = _playerController.ScreenToWorldPoint(_mousePosition);
        _handleDirection = (mousePositionWorld - playerPosition).normalized;
        
        _handlePosition = playerPosition + _handleDirection * _handleDistance;
    }

    //private void OnDrawGizmosSelected()
    //{
    //    if (!Application.isPlaying) return;
        
    //    var playerPosition = _playerController.CenterPosition + Vector3.up * _handleHeight;
        
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(playerPosition, _handleDistance);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(_handlePosition, .1f);
    //}
}
