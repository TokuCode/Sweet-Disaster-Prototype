using UnityEngine;

public class PoolInstanceBomb : PoolInstance
{
    [SerializeField] private ObjectBomb _objectBomb;
    
    public override void Reset()
    {
        _objectBomb.PoolReset();
        base.Reset();
    }
}
