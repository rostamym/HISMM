namespace HospitalAppointmentSystem.Infrastructure.Services.Email;

/// <summary>
/// Email configuration settings
/// </summary>
public class EmailSettings
{
    /// <summary>
    /// Email sending mode: "Smtp" for production, "File" for development
    /// </summary>
    public string Mode { get; set; } = "File";

    /// <summary>
    /// Sender email address
    /// </summary>
    public string FromEmail { get; set; } = string.Empty;

    /// <summary>
    /// Sender display name
    /// </summary>
    public string FromName { get; set; } = string.Empty;

    /// <summary>
    /// SMTP server address
    /// </summary>
    public string SmtpServer { get; set; } = string.Empty;

    /// <summary>
    /// SMTP server port (usually 587 for TLS or 465 for SSL)
    /// </summary>
    public int SmtpPort { get; set; } = 587;

    /// <summary>
    /// SMTP username for authentication
    /// </summary>
    public string SmtpUsername { get; set; } = string.Empty;

    /// <summary>
    /// SMTP password for authentication
    /// </summary>
    public string SmtpPassword { get; set; } = string.Empty;

    /// <summary>
    /// Enable SSL/TLS encryption
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// Directory path for file-based email output (development mode)
    /// </summary>
    public string FileOutputPath { get; set; } = "emails";
}
