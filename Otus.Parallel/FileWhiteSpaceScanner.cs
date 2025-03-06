using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Otus.Parallel;

public static class FileWhiteSpaceScanner
{
    public static ScanResult ScanFile(string fileName)
    {
        var pattern = @"\s+";
        var regex = new Regex(pattern);
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        string document;
        try
        {
            using var reader = new StreamReader(fileName);
            document = reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            throw new IOException($"Ошибка при чтении файла {fileName}: {ex.Message}");
        }

        var matches = regex.Matches(document);
        
        stopwatch.Stop();

        return new ScanResult 
        { 
            FileName = fileName, 
            WhiteSpacesCount = matches.Count, 
            ProcessingTime = stopwatch.Elapsed 
        };
    }

    public static async Task<ScanResult> ScanFileAsync(string fileName)
    {
        var pattern = @"\s+";
        var regex = new Regex(pattern);
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        string document;
        try
        {
            using var reader = new StreamReader(fileName);
            document = await reader.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            throw new IOException($"Ошибка при чтении файла {fileName}: {ex.Message}");
        }

        var matches = regex.Matches(document);

        stopwatch.Stop();

        return new ScanResult
        {
            FileName = fileName,
            WhiteSpacesCount = matches.Count,
            ProcessingTime = stopwatch.Elapsed
        };
    }

    public static IEnumerable<ScanResult> ScanDirectoryFiles(string directory)
    {
        if (!Directory.Exists(directory))
            throw new IOException($"Directory {directory} not found");

        var results = new ConcurrentBag<ScanResult>();
        var directoryFiles = Directory.GetFiles(directory);

        // Параллельная обработка всех файлов в указанной директории
        System.Threading.Tasks.Parallel.ForEach(directoryFiles, fileName =>
        {
            var scanResult = ScanFile(fileName);
            results.Add(scanResult);
        });

        return results;
    }
}
