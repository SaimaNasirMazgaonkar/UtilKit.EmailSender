using System.Collections.Generic;
using System.Threading.Tasks;

namespace UtilKit.EmailSender;

public interface IEmailSender
{
  Task SendAsync(string toEmail, string subject, string htmlBody);
  Task SendAsync(string toEmail, string subject, string htmlBody, List<string>? ccEmails, List<string>? bccEmails);
  Task SendAsync(List<string> toEmails, string subject, string htmlBody, List<string>? ccEmails, List<string>? bccEmails);
}
