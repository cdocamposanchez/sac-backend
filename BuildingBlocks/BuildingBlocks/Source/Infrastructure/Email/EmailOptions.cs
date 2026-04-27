namespace BuildingBlocks.Source.Infrastructure.Email;

public class EmailOptions
{
    public const string SectionName = "Email";

    public string SmtpHost { get; set; } = "smtp.gmail.com";
    public int SmtpPort { get; set; } = 587;
    public bool UseStartTls { get; set; } = true;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = "Sistema Académico Cayzedo (SAC)";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
