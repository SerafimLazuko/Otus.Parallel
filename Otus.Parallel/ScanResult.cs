namespace Otus.Parallel;

public record ScanResult()
{
    public int WhiteSpacesCount {  get; set; }
    public string FileName { get; set; }
    public TimeSpan ProcessingTime { get; set; }
}
