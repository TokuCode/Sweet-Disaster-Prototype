using UnityEngine;
using UnityEngine.UI;

public class ThrowUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    
    [Header("UI Elements")]
    [SerializeField] private Image _chargeFillImage;
    
    [Header("Values")]
    [SerializeField] private Color _throwReady;
    [SerializeField] private Color _throwing;
    [SerializeField] private Color _throwOnCooldown;
    
    private void Update()
    {
        UpdateFillValue();
        UpdateFillColor();
    }

    private void UpdateFillValue()
    {
        if (_playerController != null)
        {
            if (_playerController.IsThrowing)
                _chargeFillImage.fillAmount =
                    Mathf.Clamp01(_playerController.ThrowChargeTimer / _playerController.ThrowChargeTimeSeconds);
            else _chargeFillImage.fillAmount = 1;
        }
    }

    private void UpdateFillColor()
    {
        if (_playerController != null)
        {
            if (_playerController.IsThrowing)
                _chargeFillImage.color = _throwing;
            else if (_playerController.IsThrowOnCooldown)
                _chargeFillImage.color = _throwOnCooldown;
            else _chargeFillImage.color = _throwReady;
        }
    }
}
