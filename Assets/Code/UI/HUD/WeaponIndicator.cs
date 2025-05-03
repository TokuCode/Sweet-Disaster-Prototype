using TMPro;
using UnityEngine;

public class WeaponIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _weaponIndicator;
    
    private void Update() => UpdateText();

    private void UpdateText()
    {
        if (_playerController == null) return;

        var weaponName = _playerController.IsShieldActive ? "Shield" :
            _playerController.IsShootingActive ? "Shoot" : "Bomb";
        _weaponIndicator.text = weaponName;
    }
}
