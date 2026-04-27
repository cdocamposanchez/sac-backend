#region

using BuildingBlocks.Source.Application.Utils;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

#endregion

namespace BuildingBlocks.Source.Infrastructure.Email;

public class SmtpEmailService(
    IOptions<EmailOptions> options,
    ILogger<SmtpEmailService> logger) : IEmailService
{
    private readonly EmailOptions options = options.Value;

    public async Task<EmailSendResult> SendAsync(
        string toAddress,
        string toName,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(toAddress))
            return new EmailSendResult(false, "Dirección destino vacía", AppDateTime.Now);

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(options.FromName, options.FromAddress));
            message.To.Add(new MailboxAddress(toName ?? string.Empty, toAddress));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = htmlBody };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();

            var secureOption = options.UseStartTls
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.SslOnConnect;

            await client.ConnectAsync(options.SmtpHost, options.SmtpPort, secureOption, cancellationToken);
            await client.AuthenticateAsync(options.Username, options.Password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            logger.LogInformation("Correo enviado con éxito a {Destino}", toAddress);
            return new EmailSendResult(true, null, AppDateTime.Now);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error enviando correo a {Destino}", toAddress);
            return new EmailSendResult(false, ex.Message, AppDateTime.Now);
        }
    }
}
