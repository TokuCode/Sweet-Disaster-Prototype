using UnityEngine;

public class PoolInstanceBullet : PoolInstance
{
    [SerializeField] private ObjectBullet _objectBullet;

    public override void Reset()
    {
        _objectBullet.PoolReset();
        base.Reset();
    }
}
