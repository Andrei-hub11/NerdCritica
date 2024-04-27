namespace NerdCritica.Domain.Utils;

public static class FormatHelper
{
    public static string FormatRuntime(int runtimeSeconds)
    {
        if (runtimeSeconds < 0)
            throw new ArgumentException("Runtime cannot be negative.", nameof(runtimeSeconds));

        TimeSpan runtimeTimeSpan = TimeSpan.FromSeconds(runtimeSeconds);

        int hours = runtimeTimeSpan.Hours;
        int minutes = runtimeTimeSpan.Minutes;

        return (hours, minutes) switch
        {
            // Caso especial: 0 horas e minutos diferentes de zero
            (0, var m) when m != 0 => $"{m} minutos de duração",

            // Caso especial: 1 hora exata
            (1, 0) => "1 hora de duração",

            // Caso especial: 1 hora e minutos diferentes de zero
            (1, var m) => $"1 hora e {m} minutos de duração",

            // Caso especial: horas exatas e zero minutos
            (var h, 0) => $"{h} horas de duração",

            // Caso geral: horas e minutos diferentes de zero
            (var h, var m) => $"{h} horas e {m} minutos de duração"
        };
 }
}

