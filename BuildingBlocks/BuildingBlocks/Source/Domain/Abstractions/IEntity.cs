namespace BuildingBlocks.Source.Domain.Abstractions;

public interface IEntity<T> : IEntity
{
    public T Id { get; set; }
}

public interface IEntity
{
    public DateTime FechaReg { get; set; }
    public DateTime? FechaMod { get; set; }
}
