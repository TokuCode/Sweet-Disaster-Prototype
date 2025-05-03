using System;

public class Process<T> : IProcess<T> where T : IEvent
{
    public int Order { get; private set; }
    public Func<T, T> ModifyFunc { get; private set; }
    public Func<T, bool> ConditionFunc { get; private set; }
    
    public Process(int order, Func<T, T> modifyFunc, Func<T, bool> conditionFunc)
    {
        Order = order;
        ModifyFunc = modifyFunc;
        ConditionFunc = conditionFunc;
    }

    public T Modify(T @event, out bool success)
    { 
        success = ConditionFunc(@event);
        return ModifyFunc(@event);  
    } 
}
