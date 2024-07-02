using NerdCritica.Domain.ObjectValues;

namespace NerdCritica.Application.Services.EmailService;

public interface IEmailService
{
    Task Send(EmailMetadata emailMetadata);
}
