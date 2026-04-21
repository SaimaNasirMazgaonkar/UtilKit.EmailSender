# UtilKit.EmailSender

A lightweight SMTP email sender for .NET. Works with Gmail, Zoho, Outlook, AWS SES — any SMTP provider. Falls back to logging if SMTP not configured.

## Install

```bash
dotnet add package UtilKit.EmailSender
```

## Quick Start

### With appsettings.json (recommended)

```json
{
  "SmtpSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "SenderEmail": "",
    "SenderPassword": "",
    "SenderName": "MyApp",
    "EnableSsl": true
  }
}
```

> ⚠️ **Never store `SenderEmail` and `SenderPassword` in appsettings.json.** Use environment variables or cloud secret managers (AWS Secrets Manager, Azure Key Vault) instead. See [Secrets Management](#cloud-secret-managers) section below.

```csharp
// Program.cs
builder.Services.AddUtilKitEmailSender(builder.Configuration);

// Or with custom section name
builder.Services.AddUtilKitEmailSender(builder.Configuration, "EmailConfig");
```

### With inline config

```csharp
builder.Services.AddUtilKitEmailSender(options =>
{
    options.Host = "smtp.gmail.com";
    options.Port = 587;
    options.SenderEmail = "your-email@gmail.com";
    options.SenderPassword = "your-app-password";
    options.SenderName = "MyApp";
});
```

### Send Email

```csharp
public class NotificationService
{
    private readonly IEmailSender _email;

    public NotificationService(IEmailSender email)
    {
        _email = email;
    }

    public async Task SendWelcome(string userEmail, string userName)
    {
        await _email.SendAsync(
            userEmail,
            "Welcome!",
            $"<h1>Hello {userName}!</h1><p>Welcome to our app.</p>"
        );
    }
}
```

### Send with CC and BCC

```csharp
await _email.SendAsync(
    "user@example.com",
    "Invoice",
    "<p>Your invoice is attached.</p>",
    ccEmails: new List<string> { "manager@example.com" },
    bccEmails: new List<string> { "archive@example.com" }
);
```

### Without DI

```csharp
var logger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger<SmtpEmailSender>();

var sender = new SmtpEmailSender(new SmtpOptions
{
    Host = "smtp.gmail.com",
    Port = 587,
    SenderEmail = "your-email@gmail.com",
    SenderPassword = "your-app-password",
    SenderName = "MyApp"
}, logger);

await sender.SendAsync("user@example.com", "Test", "<p>Hello!</p>");
```

## SMTP Providers

| Provider | Host | Port |
|----------|------|------|
| Gmail | smtp.gmail.com | 587 |
| Zoho | smtp.zoho.com | 587 |
| Outlook | smtp.office365.com | 587 |
| AWS SES | email-smtp.us-east-1.amazonaws.com | 587 |
| SendGrid | smtp.sendgrid.net | 587 |

> **Gmail:** Use an [App Password](https://myaccount.google.com/apppasswords), not your regular password.

## Fallback

If `Host` is empty or not configured, emails are **logged instead of sent**. This is useful for development — you see the email content in logs without needing SMTP.

## Environment Variables

Keep credentials out of code:

```bash
setx SmtpSettings__Host "smtp.gmail.com"
setx SmtpSettings__SenderEmail "your-email@gmail.com"
setx SmtpSettings__SenderPassword "your-app-password"
```

## Cloud Secret Managers

This package reads from `IConfiguration` — it works automatically with any configuration source. No code change needed in your app.

### AWS Secrets Manager

```bash
dotnet add package AWSSDK.SecretsManager
dotnet add package Kralizek.Extensions.Configuration.AWSSecretsManager
```

```csharp
// Program.cs — just add the source
builder.Configuration.AddSecretsManager();

// UtilKit.EmailSender reads from IConfiguration as usual
builder.Services.AddUtilKitEmailSender(builder.Configuration);
```

### Azure Key Vault

```bash
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
dotnet add package Azure.Identity
```

```csharp
builder.Configuration.AddAzureKeyVault(
    new Uri("https://your-vault.vault.azure.net/"),
    new DefaultAzureCredential());

builder.Services.AddUtilKitEmailSender(builder.Configuration);
```

### Priority Order

```
Cloud Secrets > Environment Variables > appsettings.json > Inline code
```

Higher priority sources override lower ones. Your SMTP credentials can come from any source — the package doesn't need to know.

## Disclaimer

This package is provided **"as is"** without warranty of any kind. The author(s) are not responsible for any damages, data loss, security breaches, or issues arising from the use of this package. It is the consumer's responsibility to:

- Test thoroughly in their environment before production use
- Never store SMTP credentials in source code or appsettings.json
- Use environment variables or cloud secret managers for sensitive data
- Comply with email sending regulations (CAN-SPAM, GDPR, etc.)

Use at your own risk.

## License

MIT
