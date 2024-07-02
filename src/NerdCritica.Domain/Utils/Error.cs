namespace NerdCritica.Domain.Utils;

public class Error
{
    public string Description { get; }

    public Error(string description)
    {
        Description = description;
    }
}
