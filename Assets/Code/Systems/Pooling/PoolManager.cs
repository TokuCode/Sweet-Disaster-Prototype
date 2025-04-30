using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Header("Pool Parameters")] 
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _initialPoolSize;
    [SerializeField] private int _maxPoolSize;
    [SerializeField] private int _poolSize;
    [SerializeField] private List<PoolInstance> _poolInstances;

    private void Start()
    {
        _poolSize = _initialPoolSize;
        _poolInstances = new List<PoolInstance>();
        for (int i = 0; i < _poolSize; i++)
            AddInstance();
    }

    protected virtual PoolInstance PreRequest()
    {
        var tempInstance = _poolInstances.Find(i => !i.IsActive);
        
        if(tempInstance != null)
            return tempInstance;

        if (_poolInstances.Count < _maxPoolSize)
            return AddInstance();
        
        var oldestInstance = _poolInstances
            .OrderByDescending(i => i.TimeSinceLastUse)
            .First();
        
        oldestInstance.Reset();

        return oldestInstance;
    }
    
    public PoolInstance Request()
    {
        var instance = PreRequest();
        instance.Activate();
        return instance;
    }


    protected virtual PoolInstance CreateNewInstance()
    {
        var instance = Instantiate(_prefab, transform).GetComponent<PoolInstance>();
        return instance;
    }

    private PoolInstance AddInstance()
    {
        var instance = CreateNewInstance();
        _poolInstances.Add(instance);
        return instance;
    }
}
