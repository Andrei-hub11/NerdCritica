namespace NerdCritica.Application.Services.ImageServiceConfiguration;

public class ImageServiceConfiguration : IImageServiceConfiguration
{
    public string ApiRootDirectory { get; }

    public ImageServiceConfiguration(string baseDirectory)
    {
        ApiRootDirectory = FindApiRootDirectory(baseDirectory);
    }

    private static string FindApiRootDirectory(string baseDirectory)
    {
        string apiFolderName = "NerdCritica.Api";

        // Começa a busca a partir do diretório base fornecido
        var currentDirectory = baseDirectory;

        while (!string.IsNullOrWhiteSpace(currentDirectory))
        {
            var subdirectories = Directory.GetDirectories(currentDirectory);

            foreach (var subdirectory in subdirectories)
            {
                if (Path.GetFileName(subdirectory).Equals(apiFolderName, StringComparison.OrdinalIgnoreCase))
                {
                    return subdirectory;
                }
            }

            // Vai para o diretório pai
            currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
        }

        throw new DirectoryNotFoundException($"Diretório '{apiFolderName}' não encontrado a partir de '{baseDirectory}'.");
    }
}

