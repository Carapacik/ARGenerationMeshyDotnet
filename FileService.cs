﻿public class FileStorageSettings
{
    public FileStorageSettings(string basePath)
    {
        BasePath = basePath;
    }

    public string BasePath { get; }
}

public class FileResultCommand
{
    public FileResultCommand(byte[] content, string extension)
    {
        Content = content;
        Extension = extension;
    }

    public byte[] Content { get; init; }
    public string Extension { get; init; }
}

public class SaveFileResultCommand
{
    public string RelativeUri { get; set; } = string.Empty;
}

public class FileStorageService
{
    private readonly FileStorageSettings _fileStorageSettings;

    public FileStorageService(FileStorageSettings fileStorageSettings)
    {
        _fileStorageSettings = fileStorageSettings;
    }

    public async Task<FileResultCommand> GetFile(string path)
    {
        return new FileResultCommand(
            await File.ReadAllBytesAsync($"{_fileStorageSettings.BasePath}\\{path}"),
            path.Split('.').Last());
    }

    public Task RemoveFile(string path, string fileName)
    {
        File.Delete($"{_fileStorageSettings.BasePath}\\{path}\\{fileName}");
        return Task.CompletedTask;
    }

    public async Task<SaveFileResultCommand?> SaveFile(FileResultCommand? file, string path)
    {
        if (file is null) return null;
        var fileName = $"{Guid.NewGuid().ToString()}.{file.Extension}";
        var newFilePath = $"{_fileStorageSettings.BasePath}\\{path}\\{fileName}";
        await File.WriteAllBytesAsync(newFilePath, file.Content);
        return new SaveFileResultCommand
        {
            RelativeUri = fileName
        };
    }
}