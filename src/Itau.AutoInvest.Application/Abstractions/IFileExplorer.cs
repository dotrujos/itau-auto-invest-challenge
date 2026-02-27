namespace Itau.AutoInvest.Application.Abstractions;

public interface IFileExplorer
{
    IEnumerable<FileInfo> GetPendingFiles();
    DateTime? GetDateFromFileName(string fileName);
    void DeleteFile(string fileName);
}