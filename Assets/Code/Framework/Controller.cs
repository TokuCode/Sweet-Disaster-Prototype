using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Controller : NetworkBehaviour
{
    protected List<Feature> _features; 
    
    //protected virtual void Start()
    //{
        
    //}

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        foreach (var feature in _features)
            feature.InitializeFeature(this);
    }

    protected virtual void Update()
    {
        if (!IsOwner) return;
        foreach (var feature in _features)
            feature.UpdateFeature(UpdateContext.Update);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        foreach (var feature in _features)
            feature.UpdateFeature(UpdateContext.FixedUpdate);
    }
}
