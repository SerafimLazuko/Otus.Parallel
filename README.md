# Параллельное чтение файлов и подсчет пробелов

## Описание

В рамках данного задания необходимо реализовать функции для параллельного чтения файлов и вычисления количества пробелов в них с использованием `Task` и `Parallel.ForEach`. Основная цель — оптимизировать процесс подсчета пробелов, параллельно обрабатывая несколько файлов и измеряя время выполнения кода.


## Код класса, который сканирует файлы

```csharp
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
```

## Выполнение программы

```csharp
using System.Diagnostics;

Console.WriteLine("1) Прочитать 3 файла параллельно и вычислить количество пробелов в них (через Task).");

var stopwatch = new Stopwatch();

var readText1Async = FileWhiteSpaceScanner.ScanFileAsync("../../../TestDataTask/Text1.txt");
var readText2Async = FileWhiteSpaceScanner.ScanFileAsync("../../../TestDataTask/Text2.txt");
var readText3Async = FileWhiteSpaceScanner.ScanFileAsync("../../../TestDataTask/Text3.txt");

stopwatch.Start();

var results = await Task.WhenAll(readText1Async, readText2Async, readText3Async);

stopwatch.Stop();

Console.WriteLine($"Общее время выполнения Task.WhenAll: {stopwatch.Elapsed.TotalMilliseconds} мс");
Console.WriteLine("Результаты выполнения Task.WhenAll: ");
foreach (var result in results)
{
    Console.WriteLine($"Файл: {result.FileName}, Количество пробелов: {result.WhiteSpacesCount}, Время обработки: {result.ProcessingTime.TotalMilliseconds} мс");
}

Console.WriteLine();
Console.WriteLine("2) Из папки параллельно прочитать все файлы и вычислить количество пробелов в них");

stopwatch.Reset();
stopwatch.Start();
var directoryScanResult = FileWhiteSpaceScanner.ScanDirectoryFiles("../../../TestDataParallel");
stopwatch.Stop();

Console.WriteLine($"Общее время выполнения FileWhiteSpaceScanner.ScanDirectoryFiles: {stopwatch.Elapsed.TotalMilliseconds} мс");
Console.WriteLine("Результаты выполнения FileWhiteSpaceScanner.ScanDirectoryFiles: ");
foreach (var result in directoryScanResult)
{
    Console.WriteLine($"Файл: {result.FileName}, Количество пробелов: {result.WhiteSpacesCount}, Время обработки: {result.ProcessingTime.TotalMilliseconds} мс");
}

Console.ReadKey();
```

## Результаты выполнения

1. **Прочитать 3 файла параллельно и вычислить количество пробелов в них (через Task):**

   - Общее время выполнения `Task.WhenAll`: 9,8107 мс
   - Результаты выполнения `Task.WhenAll`:
     - Файл: `../../../TestDataTask/Text1.txt`, Количество пробелов: 297, Время обработки: 3,9844 мс
     - Файл: `../../../TestDataTask/Text2.txt`, Количество пробелов: 411, Время обработки: 1,803 мс
     - Файл: `../../../TestDataTask/Text3.txt`, Количество пробелов: 490, Время обработки: 9,6786 мс

2. **Из папки параллельно прочитать все файлы и вычислить количество пробелов в них:**

   - Общее время выполнения `FileWhiteSpaceScanner.ScanDirectoryFiles`: 23,1539 мс
   - Результаты выполнения `FileWhiteSpaceScanner.ScanDirectoryFiles`:
     - Файл: `../../../TestDataParallel\Text5.txt`, Количество пробелов: 496, Время обработки: 0,6442 мс
     - Файл: `../../../TestDataParallel\Text6.txt`, Количество пробелов: 350, Время обработки: 0,3435 мс
     - Файл: `../../../TestDataParallel\Text7.txt`, Количество пробелов: 510, Время обработки: 0,2712 мс
     - Файл: `../../../TestDataParallel\Text9.txt`, Количество пробелов: 551, Время обработки: 0,6056 мс
     - Файл: `../../../TestDataParallel\Text8.txt`, Количество пробелов: 669, Время обработки: 0,6753 мс
     - Файл: `../../../TestDataParallel\Text4.txt`, Количество пробелов: 345, Время обработки: 0,6608 мс
