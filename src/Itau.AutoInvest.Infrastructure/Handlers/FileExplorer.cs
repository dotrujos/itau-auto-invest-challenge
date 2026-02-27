using System.Text.RegularExpressions;
using Itau.AutoInvest.Application.Abstractions;

namespace Itau.AutoInvest.Infrastructure.Handlers;

public class FileExplorer : IFileExplorer
{
    private readonly string _directoryPath;
    private const string FilePattern = @"COTAHIST_D(\d{8})\.TXT";

    public FileExplorer()
    {
        _directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cotacoes");

        var currentDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        
        while (currentDir != null && !currentDir.GetDirectories("src").Any())
        {
            currentDir = currentDir.Parent;
        }
        
        _directoryPath = Path.Combine(currentDir.FullName, "cotacoes");

        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);
    }
    
    public IEnumerable<FileInfo> GetPendingFiles()
    {
        var directoryInfo = new DirectoryInfo(_directoryPath);
        
        return directoryInfo.EnumerateFiles("COTAHIST_D*.TXT")
            .Where(file => Regex.IsMatch(file.Name, FilePattern))
            .OrderBy(file => file.Name); 
    }
    
    public DateTime? GetDateFromFileName(string fileName)
    {
        var match = Regex.Match(fileName, FilePattern);
        if (match.Success && DateTime.TryParseExact(match.Groups[1].Value, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out var date))
        {
            return date;
        }
        return null;
    }

    public void DeleteFile(string fileName)
    {
        throw new NotImplementedException();
    }
}