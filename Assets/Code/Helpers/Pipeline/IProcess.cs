public interface IProcess<T> where T : IEvent
{
    int Order { get; }
    T Modify(T @event, out bool success);
}
