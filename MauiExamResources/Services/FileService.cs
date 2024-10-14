﻿using MauiExamResources.Interfaces;
using MauiExamResources.Models;

namespace MauiExamResources.Services;

public class FileService(string filePath = @"C:\projects\products.json") : IFileService
{
    private readonly string _filePath = filePath;

    public ResponseResult<string> GetFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
                return new ResponseResult<string> { Success = false, Message = "File not found." };

            using var sr = new StreamReader(_filePath);
            var content = sr.ReadToEnd();

            return new ResponseResult<string> { Success = true, Result = content };
        }
        catch (Exception ex)
        {
            return new ResponseResult<string> { Success = false, Message = ex.Message };
        }
    }
    public ResponseResult<string> SaveToFile(string content)
    {
        try
        {
            using var sw = new StreamWriter(_filePath, false);
            sw.WriteLine(content);

            return new ResponseResult<string> { Success = true };
        }
        catch (Exception ex)
        {
            return new ResponseResult<string> { Success = false, Message = ex.Message };
        }
    }

}
