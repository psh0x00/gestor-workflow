namespace GestorWorkflow.Core.Interfaces;

public interface IDomainEvent
{
    DateTime OcorridoEm { get; }
    string TipoEvento { get; }
}

public interface IDomainEventHandler<T> where T : IDomainEvent
{
    Task HandleAsync(T domainEvent);
}

public interface IDomainEventDispatcher
{
    Task DispatchAsync<T>(T domainEvent) where T : IDomainEvent;
    void Subscribe<T>(IDomainEventHandler<T> handler) where T : IDomainEvent;
}