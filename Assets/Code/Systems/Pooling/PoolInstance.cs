using System;
using UnityEngine;

public abstract class PoolInstance : MonoBehaviour
{
    [Header("Pooling Parameters")]
    [SerializeField] private bool _isActive;
    public bool IsActive => _isActive;
    [SerializeField] private float _timeSinceLastUse;
    public float TimeSinceLastUse => _timeSinceLastUse;
    [SerializeField] private GameObject _renderObject;
    public GameObject RenderObject => _renderObject;

    private void Update()
    {
        if(_isActive) 
            _timeSinceLastUse += Time.deltaTime;
    }

    public virtual void Activate()
    {
        _isActive = true;
        _renderObject.SetActive(true);
    }

    public virtual void Reset()
    {
        _isActive = false;
        _timeSinceLastUse = 0f;
        _renderObject.SetActive(false);
    }
}
