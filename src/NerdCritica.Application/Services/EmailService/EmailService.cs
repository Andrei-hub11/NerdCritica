using FluentEmail.Core;
using NerdCritica.Domain.ObjectValues;

namespace NerdCritica.Application.Services.EmailService;

public class EmailService : IEmailService
{
    private readonly IFluentEmail _fluentEmail;

    public EmailService(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail
            ?? throw new ArgumentNullException(nameof(fluentEmail));
    }

    public async Task Send(EmailMetadata emailMetadata)
    {
        await _fluentEmail.To(emailMetadata.ToAddress)
            .Subject(emailMetadata.Subject)
            .Body(emailMetadata.Body)
            .SendAsync();
    }
}
