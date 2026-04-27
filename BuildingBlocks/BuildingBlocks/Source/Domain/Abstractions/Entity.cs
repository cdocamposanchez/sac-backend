namespace BuildingBlocks.Source.Domain.Abstractions;

public abstract class Entity<T> : IEntity<T>
{
    private DateTime? fechaMod;
    private DateTime fechaReg;

    public T Id { get; set; } = default!;

    public DateTime FechaReg
    {
        get => fechaReg;
        set => fechaReg = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    public DateTime? FechaMod
    {
        get => fechaMod;
        set => fechaMod = value.HasValue
            ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
            : null;
    }
}
