using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace UtilKit.EmailSender;

public class SmtpEmailSender : IEmailSender
{
  private readonly SmtpOptions _options;
  private readonly ILogger<SmtpEmailSender> _logger;

  public SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger)
  {
    _options = options.Value;
    _logger = logger;
  }

  public SmtpEmailSender(SmtpOptions options, ILogger<SmtpEmailSender> logger)
  {
    _options = options;
    _logger = logger;
  }

  public Task SendAsync(string toEmail, string subject, string htmlBody)
  {
    return SendAsync(new List<string> { toEmail }, subject, htmlBody, null, null);
  }

  public Task SendAsync(string toEmail, string subject, string htmlBody, List<string>? ccEmails, List<string>? bccEmails)
  {
    return SendAsync(new List<string> { toEmail }, subject, htmlBody, ccEmails, bccEmails);
  }

  public async Task SendAsync(List<string> toEmails, string subject, string htmlBody, List<string>? ccEmails, List<string>? bccEmails)
  {
    if (string.IsNullOrEmpty(_options.Host))
    {
      _logger.LogWarning("SMTP not configured. Email to {Emails}: {Subject} — {Body}", string.Join(", ", toEmails), subject, htmlBody);
      return;
    }

    using var message = new MailMessage();
    message.From = new MailAddress(_options.SenderEmail, _options.SenderName);

    foreach (var to in toEmails)
      message.To.Add(to);

    message.Subject = subject;
    message.Body = htmlBody;
    message.IsBodyHtml = true;

    if (ccEmails != null)
    {
      foreach (var cc in ccEmails)
        message.CC.Add(cc);
    }

    if (bccEmails != null)
    {
      foreach (var bcc in bccEmails)
        message.Bcc.Add(bcc);
    }

    using var client = new SmtpClient(_options.Host, _options.Port);
    client.Credentials = new NetworkCredential(_options.SenderEmail, _options.SenderPassword);
    client.EnableSsl = _options.EnableSsl;

    await client.SendMailAsync(message);
    _logger.LogInformation("Email sent to {Emails}: {Subject}", string.Join(", ", toEmails), subject);
  }
}
