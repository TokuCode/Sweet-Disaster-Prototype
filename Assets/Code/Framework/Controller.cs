using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    protected List<Feature> _features; 
    
    protected virtual void Start()
    {
        foreach (var feature in _features)
            feature.InitializeFeature(this);
    }

    protected virtual void Update()
    {
        foreach (var feature in _features)
            feature.UpdateFeature(UpdateContext.Update);
    }

    private void FixedUpdate()
    {
        foreach (var feature in _features)
            feature.UpdateFeature(UpdateContext.FixedUpdate);
    }
}
