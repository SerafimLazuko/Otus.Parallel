using Otus.Parallel;
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