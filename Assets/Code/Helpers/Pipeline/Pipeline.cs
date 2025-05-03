using System.Collections.Generic;
using System.Linq;

public class Pipeline<T> where T : IEvent
{
    private readonly List<IProcess<T>> bindings = new List<IProcess<T>>();
        
    public void Register(IProcess<T> binding) => bindings.Add(binding);
    public void Deregister(IProcess<T> binding) => bindings.Remove(binding);
        
    public T Process(T @event, out bool success)
    {
        success = false;
        var orderedBindings = bindings.OrderByDescending(b => b.Order);
            
        T currentEvent = @event;
        foreach (var binding in orderedBindings)
        {
            currentEvent = binding.Modify(currentEvent, out success);
            if (!success)
                break;
        }

        return currentEvent;
    }
        
    public void Clear() => bindings.Clear();
}