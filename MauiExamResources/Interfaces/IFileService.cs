using MauiExamResources.Models;

namespace MauiExamResources.Interfaces;

public interface IFileService
{
    public ResponseResult<string> SaveToFile(string content);
    public ResponseResult<string> GetFromFile();
}
