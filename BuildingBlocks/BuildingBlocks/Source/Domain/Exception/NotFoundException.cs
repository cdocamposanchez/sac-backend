namespace BuildingBlocks.Source.Domain.Exception;

public class NotFoundException : System.Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string name, object key) : base($"Entidad \"{name}\" ({key}) no encontrado.")
    {
    }

    public NotFoundException(string name, string name2, object key) : base(
        $"Entidad \"{name}\" con {name2} ({key}) no encontrado.")
    {
    }
}
