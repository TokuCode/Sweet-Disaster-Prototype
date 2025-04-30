using UnityEngine;

public class PoolInstanceBullet : PoolInstance
{
    [Header("References")]
    [SerializeField] private ObjectBullet _objectBullet;

    public override void Reset()
    {
        _objectBullet.PoolReset();
        base.Reset();
    }
}
