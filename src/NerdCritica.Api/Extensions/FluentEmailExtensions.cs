namespace NerdCritica.Api.Extensions;

public static class FluentEmailExtensions
{
    public static void AddFluentEmail(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var emailSettings = configuration.GetSection("Smtp");

        var defaultFromEmail = emailSettings["DefaultFromEmail"];
        var host = emailSettings["Host"];
        var port = emailSettings.GetValue<int>("Port");
        var userName = emailSettings["UserName"];
        var password = emailSettings["Password"];

        services.AddFluentEmail(defaultFromEmail)
            .AddSmtpSender(host, port, userName, password);
    }
}
