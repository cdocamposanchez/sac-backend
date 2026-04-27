namespace BuildingBlocks.Source.Domain.Exception;

public class DomainException(string message, string type)
    : System.Exception($"Excepción de dominio en la clase '{type}': {message}");
