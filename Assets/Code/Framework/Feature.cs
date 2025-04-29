using UnityEngine;

public abstract class Feature : MonoBehaviour
{
    public abstract void InitializeFeature(Controller controller);
    public abstract void UpdateFeature(UpdateContext context);
}
