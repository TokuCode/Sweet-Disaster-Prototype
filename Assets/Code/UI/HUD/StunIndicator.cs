using UnityEngine;
using UnityEngine.UI;

public class StunIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    
    [Header("UI Elements")]
    [SerializeField] private Image _stunIndicatorImage;
    
    [Header("Stun Parameters")]
    [SerializeField] private Color _stunColor;
    [SerializeField] private Color _normalColor;

    private void Update() => UpdateColor();

    private void UpdateColor()
    {
        _stunIndicatorImage.color = _playerController.IsStunned ? _stunColor : _normalColor;
    }
}
