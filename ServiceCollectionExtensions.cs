using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UtilKit.EmailSender;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddUtilKitEmailSender(this IServiceCollection services, IConfiguration configuration, string sectionName = "SmtpSettings")
  {
    services.Configure<SmtpOptions>(configuration.GetSection(sectionName));
    services.AddScoped<IEmailSender, SmtpEmailSender>();
    return services;
  }

  public static IServiceCollection AddUtilKitEmailSender(this IServiceCollection services, Action<SmtpOptions> configure)
  {
    services.Configure(configure);
    services.AddScoped<IEmailSender, SmtpEmailSender>();
    return services;
  }
}
