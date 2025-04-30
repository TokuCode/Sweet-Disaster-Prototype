using UnityEngine;
using TMPro;

public class AmmoGauge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _ammoText;

    private void Update() => UpdateText();

    private void UpdateText()
    {
        if (_playerController != null)
        {
            if (_playerController.IsReloading)
            {
                _ammoText.text = "Reloading...";
                return;
            }
            
            _ammoText.text = $"{_playerController.CurrentAmmo}/{_playerController.MagazineSize}";
        } else _ammoText.text = string.Empty;
    }
}
