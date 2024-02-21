using System.Text;

namespace NerdCritica.Api.Utils.Helper;

public class ExceptionDetailsHelper
{
    public static string GetExceptionDetails(Exception ex, HttpContext context, int statusCode)
    {
        var exceptionDetails = new StringBuilder();
        exceptionDetails.AppendLine($"Erro ao processar a solicitação na rota '{context.Request.Path}'.");
        exceptionDetails.AppendLine($"Código HTTP: {statusCode}");
        exceptionDetails.AppendLine($"Mensagem de erro: {ex.Message}");
        exceptionDetails.AppendLine($"Detalhes da exceção: {ex.ToString()}");
        return exceptionDetails.ToString();
    }
}
