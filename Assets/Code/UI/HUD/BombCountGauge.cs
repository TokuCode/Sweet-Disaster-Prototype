using TMPro;
using UnityEngine;

public class BombCountGauge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _bombCountText;
    
    private void Update() => UpdateText();

    private void UpdateText()
    {
        if (_playerController != null)
        {
            _bombCountText.text = $"#{_playerController.BombCount}";
        }
    }
}
