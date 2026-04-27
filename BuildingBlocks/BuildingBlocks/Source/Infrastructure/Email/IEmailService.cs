namespace BuildingBlocks.Source.Infrastructure.Email;

public record EmailSendResult(bool Success, string? ErrorMessage, DateTime SentAt);

public interface IEmailService
{
    /// <summary>
    /// Envía un correo electrónico de manera individual.
    /// </summary>
    Task<EmailSendResult> SendAsync(
        string toAddress,
        string toName,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default);
}
