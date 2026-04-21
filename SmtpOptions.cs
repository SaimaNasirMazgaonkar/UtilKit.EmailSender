namespace UtilKit.EmailSender;

public class SmtpOptions
{
  public string Host { get; set; } = string.Empty;
  public int Port { get; set; } = 587;
  public string SenderEmail { get; set; } = string.Empty;
  public string SenderPassword { get; set; } = string.Empty;
  public string SenderName { get; set; } = string.Empty;
  public bool EnableSsl { get; set; } = true;
}
