using UnityEngine;
using UnityEngine.UI;

public class StaminaGauge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    
    [Header("UI Elements")]
    [SerializeField] private Image _staminaBar;
    
    [Header("Stamina Parameters")]
    [SerializeField] private Color _idleColor;
    [SerializeField] private Color _depletedColor;

    private void Update()
    { 
        UpdateSlider();  
        UpdateColor();
    } 

    private void UpdateSlider()
    {
        if (_playerController == null) return;

        var stamina = _playerController.ShieldStamina;
        var maxStamina = _playerController.MaxShieldStamina;

        _staminaBar.fillAmount = Mathf.Clamp01(stamina / maxStamina);
    }

    private void UpdateColor()
    {
        if (_playerController == null) return;

        _staminaBar.color = _playerController.IsStaminaDepleted ? _depletedColor : _idleColor;
    }
}
