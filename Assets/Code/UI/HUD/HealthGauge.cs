using TMPro;
using UnityEngine;

public class HealthGauge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _healthText;
    
    [Header("Health Parameters")]
    [SerializeField] private Color _idleColor;
    [SerializeField] private Color _overshootColor;

    private void Update() => UpdateText();

    private void UpdateText()
    {
        if (_playerController == null) return;
        
        var health = _playerController.Health;
        var baseHealth = _playerController.BaseHealth;
        
        _healthText.text = $"{health:N0}%";
        _healthText.color = health < baseHealth ? _idleColor : _overshootColor;
    }
}
