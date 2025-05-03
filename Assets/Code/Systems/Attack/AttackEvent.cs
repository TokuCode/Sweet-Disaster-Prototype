using UnityEngine;

public class AttackEvent : IEvent
{
    public Vector3 SourcePosition;
    public float DamagePercentage;
    public int KnockbackLevel;
}
